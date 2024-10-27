﻿#pragma warning disable HAA0301 // Closure Allocation Source
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
#pragma warning disable HAA0401 // Possible allocation of reference type enumerator
using EvDb.SourceGenerator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace EvDb.SourceGenerator;

[Generator]
public partial class ViewGenerator : BaseGenerator
{
    internal const string VIEW_TYPE_ATT = "EvDbViewTypeAttribute";
    protected override string EventTargetAttribute { get; } = VIEW_TYPE_ATT;

    #region OnGenerate

    protected override void OnGenerate(
            SourceProductionContext context,
            Compilation compilation,
            INamedTypeSymbol typeSymbol,
            TypeDeclarationSyntax syntax,
            CancellationToken cancellationToken)
    {
        context.ThrowIfNotPartial(typeSymbol, syntax);

        StringBuilder builder = new StringBuilder();

        string viewOriginName = typeSymbol.Name;

        #region stateType = .., eventType = .., fullName = ..

        string type = typeSymbol.ToType(syntax, cancellationToken);
        string viewClassName = $"EvDb{viewOriginName}";
        if (!viewClassName.EndsWith("View"))
            viewClassName = $"{viewClassName}View";

        AttributeData att = typeSymbol.GetAttributes()
                                  .First(att => att.AttributeClass?.Name == EventTargetAttribute);
        ImmutableArray<ITypeSymbol> args = att.AttributeClass?.TypeArguments ?? ImmutableArray<ITypeSymbol>.Empty;
        string stateType = args[0].ToDisplayString();
        ITypeSymbol eventTypeSymbol = args[1];

        #endregion //  stateType = .., eventType = .., fullName = ..

        if (!att.TryGetValue("name", out string name))
        {
            context.Throw(EvDbErrorsNumbers.MissingViewName, $"{viewOriginName}: View name is missing", syntax);
        }

        #region var eventsPayloads = from a in eventTypeSymbol.GetAttributes() ...

        var eventsPayloads = from a in eventTypeSymbol.GetAttributes()
                             let cls = a.AttributeClass!
                             where cls != null
                             let text = cls.Name
                             where text == EventTypesGenerator.EventTarget
                             let payloadType = cls.TypeArguments.First()
                             let payloadAtt = payloadType.GetAttributes().First(m => m.AttributeClass?.Name.StartsWith("EvDbDefinePayload") ?? false)
                             let eventTypeValue = payloadAtt.ConstructorArguments.First().Value?.ToString()
                             let fullName = cls?.ToString()
                             let genStart = fullName.IndexOf('<') + 1
                             let genLen = fullName.Length - genStart - 1
                             let generic = fullName.Substring(genStart, genLen)
                             let attName = a.ConstructorArguments.FirstOrDefault().Value
                             select (Type: generic, Key: eventTypeValue);
        eventsPayloads = eventsPayloads.ToArray(); // run once

        #endregion // var eventsPayloads = from a in eventTypeSymbol.GetAttributes() ...

        #region ViewBase

        #region var eventsPayloads = ...

        #endregion // var eventsPayloads = ...

        var foldAbstracts = eventsPayloads.Select(p =>
                $"""
                    protected virtual {stateType} Fold(
                            {stateType} state,
                            {p.Type} payload,
                            IEvDbEventMeta meta) => state;

                """);

        var foldMap = eventsPayloads.Select(p =>
                $$"""
                    case "{{p.Key}}":
                            {
                                var payload = c.GetData<{{p.Type}}>(_options);
                                State = Fold(State, payload, e);
                                break;
                            }
                        
                """);

        builder.ClearAndAppendHeader(syntax, typeSymbol);
        builder.AppendLine("using Microsoft.Extensions.Logging;");
        builder.AppendLine();

        builder.DefaultsOnType(typeSymbol);
        builder.AppendLine($$"""
                    public abstract class {{viewClassName}}Base: 
                        EvDbView<{{stateType}}>
                    {                                
                        public const string ViewName = "{{name}}";
                    
                        #region Ctor

                        protected {{viewClassName}}Base(
                            EvDbStreamAddress address,
                            IEvDbStorageSnapshotAdapter storageAdapter,
                            TimeProvider timeProvider,
                            ILogger logger,
                            JsonSerializerOptions? options):
                                base(new EvDbViewAddress(address, ViewName), 
                                EvDbStoredSnapshot.Empty,
                                storageAdapter, 
                                timeProvider,
                                logger,
                                options)
                        {
                        }

                        protected {{viewClassName}}Base(
                            EvDbStreamAddress address,
                            IEvDbStorageSnapshotAdapter storageAdapter, 
                            TimeProvider timeProvider,
                            ILogger logger,
                            EvDbStoredSnapshot snapshot, 
                            JsonSerializerOptions? options):
                                base(
                                    new EvDbViewAddress(address, ViewName), 
                                    snapshot,
                                    storageAdapter,
                                    timeProvider,
                                    logger,
                                    options)
                        {
                        }

                        #endregion // Ctor

                        #region OnFoldEvent

                        protected sealed override void OnFoldEvent(EvDbEvent e)
                        {
                            IEvDbEventConverter c = e;
                            switch (e.EventType)
                            {
                            {{string.Join("", foldMap)}}
                                default:
                                    throw new NotSupportedException(e.EventType);
                            }
                        }

                        #endregion // OnFoldEvent

                        #region Fold

                    {{string.Join("", foldAbstracts)}}
                        #endregion // Fold
                    }
                    """);
        context.AddSource(typeSymbol.StandardPathIgnoreSymbolName($"{viewClassName}Base"), builder.ToString());

        #endregion // ViewBase

        builder.Clear();

        #region View

        builder.ClearAndAppendHeader(syntax, typeSymbol);
        builder.AppendLine("using Microsoft.Extensions.Logging;");
        builder.AppendLine();

        builder.DefaultsOnType(typeSymbol);
        builder.AppendLine($$"""
                    partial {{type}} {{viewOriginName}}: {{viewClassName}}Base
                    { 
                        public {{viewOriginName}}(
                            EvDbStreamAddress address,
                            IEvDbStorageSnapshotAdapter storageAdapter, 
                            TimeProvider timeProvider,
                            ILogger logger,
                            JsonSerializerOptions? options):
                                    base (
                                        address, 
                                        storageAdapter, 
                                        timeProvider, 
                                        logger,
                                        options)
                        {
                        }

                        public {{viewOriginName}}(
                            EvDbStreamAddress address,
                            IEvDbStorageSnapshotAdapter storageAdapter, 
                            TimeProvider timeProvider,
                            ILogger logger,
                            EvDbStoredSnapshot snapshot, 
                            JsonSerializerOptions? options):
                                base (
                                    address,
                                    storageAdapter,
                                    timeProvider,
                                    logger,
                                    snapshot,
                                    options)
                        {
                        }
                    }
                    """);
        context.AddSource(typeSymbol.StandardPath(), builder.ToString());

        #endregion // View
    }

    #endregion // OnGenerate
}