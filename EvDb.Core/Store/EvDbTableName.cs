using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace VogenTableName;

[ExcludeFromCodeCoverage]
[JsonConverter(typeof(EvDbTableNameSystemTextJsonConverter))]
[TypeConverter(typeof(EvDbTableNameTypeConverter))]
[DebuggerTypeProxyAttribute(typeof(EvDbTableNameDebugView))]
[DebuggerDisplayAttribute("Underlying type: string, Value = { _value }")]
public partial struct EvDbTableName :
    IEquatable<EvDbTableName>,
    IEquatable<string>,
    IComparable<EvDbTableName>,
    IComparable,
    IParsable<EvDbTableName>
{
    private const string DEFAULT_TABLE_NAME = "ev-db-topic";

    public static readonly EvDbTableName Default = new EvDbTableName(DEFAULT_TABLE_NAME);

    #region Validation

    [GeneratedRegex(@"^[A-Za-z][A-Za-z0-9_]*(?:-[A-Za-z0-9_]+)*$", RegexOptions.Compiled | RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex Validator();

    private static Validation Validate(string value) => Validator().IsMatch(value) switch
    {

        true => Validation.Ok,
        _ => Validation.Invalid("The table name must only contain uppercase letters (A-Z), lowercase letters (a-z), digits (0-9), and underscores (_). Hyphens (-) are allowed but cannot appear consecutively.")
    };

    #endregion //  Validation

    #region StackTrace? _stackTrace = null!;

#if DEBUG
    private readonly StackTrace? _stackTrace = null!;
#endif

    #endregion //  StackTrace? _stackTrace = null!;

    #region readonly string Value { get; }

    private readonly string? _value;
    /// <summary>
    /// Gets the underlying <see cref = "string"/> value if set, otherwise a <see cref = "ValueObjectValidationException"/> is thrown.
    /// </summary>
    public readonly string Value
    {
        [DebuggerStepThroughAttribute]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            EnsureInitialized();
            return _value!;
        }
    }

    #endregion //  readonly string Value { get; }

    #region Ctor

    [DebuggerStepThroughAttribute]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EvDbTableName()
    {
#if DEBUG
        _stackTrace = new StackTrace();
#endif
        _isInitialized = false;
        _value = default;
    }

    [DebuggerStepThroughAttribute]
    private EvDbTableName(string value)
    {
        _value = value;
        _isInitialized = true;
    }

    #endregion //  Ctor

    #region TryFrom / From

    /// <summary>
    /// Builds an instance from the provided underlying type.
    /// </summary>
    /// <param name = "value">The underlying type.</param>
    /// <returns>An instance of this type.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EvDbTableName From(string value)
    {
        var validation = EvDbTableName.Validate(value);
        if (validation != Validation.Ok)
        {
            ThrowHelper.ThrowWhenValidationFails(validation);
        }

        return new EvDbTableName(value);
    }

    /// <summary>
    /// Tries to build an instance from the provided underlying type.
    /// If a normalization method is provided, it will be called.
    /// If validation is provided, and it fails, false will be returned.
    /// </summary>
    /// <param name = "value">The underlying type.</param>
    /// <param name = "vo">An instance of the value object.</param>
    /// <returns>True if the value object can be built, otherwise false.</returns>
    public static bool TryFrom(
                            [NotNullWhen(true)]
                            string? value,
                            [MaybeNullWhen(false)]
                            out EvDbTableName vo)
    {
        if (value is null)
        {
            vo = default;
            return false;
        }

        var validation = EvDbTableName.Validate(value);
        if (validation != Validation.Ok)
        {
            vo = default!;
            return false;
        }

        vo = new EvDbTableName(value);
        return true;
    }

    /// <summary>
    /// Tries to build an instance from the provided underlying value.
    /// If a normalization method is provided, it will be called.
    /// If validation is provided, and it fails, an error will be returned.
    /// </summary>
    /// <param name = "value">The primitive value.</param>
    /// <returns>A <see cref = "ValueObjectOrError{T}"/> containing either the value object, or an error.</returns>
    public static ValueObjectOrError<EvDbTableName> TryFrom(string value)
    {
        if (value is null)
        {
            return new ValueObjectOrError<EvDbTableName>(Validation.Invalid("The value provided was null"));
        }

        var validation = EvDbTableName.Validate(value);
        if (validation != Validation.Ok)
        {
            return new ValueObjectOrError<EvDbTableName>(validation);
        }

        return new ValueObjectOrError<EvDbTableName>(new EvDbTableName(value));
    }

    #endregion //  TryFrom / From

    #region Initialization

    private readonly bool _isInitialized;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsInitialized() => _isInitialized;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly void EnsureInitialized()
    {
        if (!IsInitialized())
        {
#if DEBUG
            ThrowHelper.ThrowWhenNotInitialized(_stackTrace);
#else
            ThrowHelper.ThrowWhenNotInitialized();
#endif
        }
    }

    #endregion //  Initialization

    // only called internally when something has been deserialized into
    // its primitive type.
    private static EvDbTableName __Deserialize(string value)
    {
        if (value == Default.Value)
            return Default;
        var validation = EvDbTableName.Validate(value);
        if (validation != Validation.Ok)
        {
            ThrowHelper.ThrowWhenValidationFails(validation);
        }

        return new EvDbTableName(value);
    }

    #region Equals / CompaareTo / GetHashCode

    public readonly bool Equals(EvDbTableName other)
    {
        // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
        // We treat anything uninitialized as not equal to anything, even obj uninitialized instances of this type.
        if (!IsInitialized() || !other.IsInitialized())
            return false;
        return EqualityComparer<string>.Default.Equals(Value, other.Value);
    }

    public bool Equals(EvDbTableName other, IEqualityComparer<EvDbTableName> comparer)
    {
        return comparer.Equals(this, other);
    }

    public readonly bool Equals(string? primitive)
    {
        return Value.Equals(primitive);
    }

    public readonly bool Equals(string? primitive, StringComparer comparer)
    {
        return comparer.Equals(Value, primitive);
    }

    public readonly override bool Equals(Object? obj)
    {
        return obj is EvDbTableName && Equals((EvDbTableName)obj);
    }

    public int CompareTo(EvDbTableName other) => Value.CompareTo(other.Value);
    public int CompareTo(object? obj)
    {
        if (obj is null)
            return 1;
        if (obj is EvDbTableName x)
            return CompareTo(x);
        ThrowHelper.ThrowArgumentException("Cannot compare to object as it is not of type EvDbTableName", nameof(obj));
        return 0;
    }

    public readonly override Int32 GetHashCode()
    {
        return EqualityComparer<string>.Default.GetHashCode(Value);
    }

    #endregion //  Equals / CompaareTo / GetHashCode

    #region Operator Overloads

    public static implicit operator string(EvDbTableName vo) => vo._value!;
    public static implicit operator EvDbTableName(string value)
    {
        return EvDbTableName.From(value);
    }

    public static bool operator ==(EvDbTableName left, EvDbTableName right) => left.Equals(right);
    public static bool operator !=(EvDbTableName left, EvDbTableName right) => !(left == right);
    public static bool operator ==(EvDbTableName left, string? right) => left.Value.Equals(right);
    public static bool operator ==(string? left, EvDbTableName right) => right.Value.Equals(left);
    public static bool operator !=(string? left, EvDbTableName right) => !(left == right);
    public static bool operator !=(EvDbTableName left, string? right) => !(left == right);

    public static bool operator <(EvDbTableName left, EvDbTableName right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(EvDbTableName left, EvDbTableName right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(EvDbTableName left, EvDbTableName right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(EvDbTableName left, EvDbTableName right)
    {
        return left.CompareTo(right) >= 0;
    }

    #endregion //  Operator Overloads

    #region Parse / TryParse

    /// <summary>
    /// </summary>
    /// <returns>
    /// True if the value passes any validation (after running any optional normalization).
    /// </returns>
    public static bool TryParse(
        [NotNullWhen(true)]
        string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)]
        out EvDbTableName result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        var validation = EvDbTableName.Validate(s);
        if (validation != Validation.Ok)
        {
            result = default;
            return false;
        }

        result = new EvDbTableName(s);
        return true;
    }

    /// <summary>
    /// </summary>
    /// <returns>
    /// The value created via the <see cref = "From(string)"/> method.
    /// </returns>
    public static EvDbTableName Parse(string s, IFormatProvider? provider)
    {
        return From(s!);
    }

    #endregion //  Parse / TryParse

    /// <summary>Returns the string representation of the underlying <see cref = "string"/>.</summary>
    public readonly override string ToString() =>
                    IsInitialized() ? Value.ToString() : "[UNINITIALIZED]";

    #region EvDbTableNameSystemTextJsonConverter

#nullable disable
    /// <summary>
    /// Converts a EvDbTableName to or from JSON.
    /// </summary>
    public class EvDbTableNameSystemTextJsonConverter : JsonConverter<EvDbTableName>
    {
        public override EvDbTableName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return EvDbTableName.__Deserialize(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, EvDbTableName value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }

        public override EvDbTableName ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return EvDbTableName.__Deserialize(reader.GetString());
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, EvDbTableName value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.Value);
        }
    }

#nullable restore

    #endregion //  EvDbTableNameSystemTextJsonConverter

#nullable disable
    #region EvDbTableNameTypeConverter

    class EvDbTableNameTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
        {
            var stringValue = value as string;
            if (stringValue is not null)
            {
                return EvDbTableName.__Deserialize(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
        {
            if (value is EvDbTableName idValue && destinationType == typeof(string))
            {
                return idValue.Value;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    #endregion //  EvDbTableNameTypeConverter
#nullable restore

    #region Validation

    public class Validation
    {
        public string ErrorMessage { get; }

        /// <summary>
        /// Contains data related to validation.
        /// </summary>
        public Dictionary<object, object>? Data { get; private set; } = null;

        public static readonly Validation Ok = new Validation(string.Empty);

        private Validation(string reason) => ErrorMessage = reason;

        public static Validation Invalid(string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
            {
                return new Validation("[none provided]");
            }

            return new Validation(reason);
        }

        /// <summary>
        /// Adds the specified data to the validation.
        /// This data will be copied to the Data property of the thrown Exception.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>Validation.</returns>
        public Validation WithData(object key, object value)
        {
            Data ??= new();
            Data[key] = value;
            return this;
        }
    }

    #endregion //  Validation

#nullable disable

    #region ValueObjectValidationException

    [Serializable]
    public class ValueObjectValidationException : Exception
    {
        public ValueObjectValidationException()
        {
        }

        public ValueObjectValidationException(string message) : base(message)
        {
        }

        public ValueObjectValidationException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    #endregion //  ValueObjectValidationException

    #region EvDbTableNameDebugView

#nullable restore
#nullable disable
#pragma warning disable S3453 // Classes should not have only "private" constructors
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable S1144 // Unused private types or members should be removed
    internal sealed class EvDbTableNameDebugView
    {
        private readonly EvDbTableName _t;
        EvDbTableNameDebugView(EvDbTableName t)
        {
            _t = t;
        }

        public bool IsInitialized => _t.IsInitialized();
        public string UnderlyingType => "string";
        public string Value => _t.IsInitialized() ? _t._value.ToString() : "[not initialized]";
#if DEBUG
        public string CreatedWith => _t._stackTrace?.ToString() ?? "the From method";
#endif
        public string Conversions => @"Default";
    }
#pragma warning restore S3453 // Classes should not have only "private" constructors
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore S1144 // Unused private types or members should be removed
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

    #endregion //  EvDbTableNameDebugView

    #region ThrowHelper

#nullable restore
    static class ThrowHelper
    {
        [DoesNotReturnAttribute]
        internal static void ThrowInvalidOperationException(string message) => throw new InvalidOperationException(message);
        [DoesNotReturnAttribute]
        internal static void ThrowArgumentException(string message, string arg) => throw new ArgumentException(message, arg);
        [DoesNotReturnAttribute]
        internal static void ThrowWhenCreatedWithNull() => throw new ArgumentException("Cannot create a value object with null.");
        [DoesNotReturnAttribute]
        internal static void ThrowWhenNotInitialized() => throw new ArgumentException("Use of uninitialized Value Object.");
        [DoesNotReturnAttribute]
        internal static void ThrowWhenNotInitialized(StackTrace? stackTrace) => throw new ArgumentException("Use of uninitialized Value Object at: " + stackTrace);
        [DoesNotReturnAttribute]
        internal static void ThrowWhenValidationFails(Validation validation)
        {
            var ex = new ValueObjectValidationException(validation.ErrorMessage);
            if (validation.Data is not null)
            {
                foreach (var kvp in validation.Data)
                {
                    ex.Data[kvp.Key] = kvp.Value;
                }
            }

            throw ex;
        }
    }

    #endregion //  ThrowHelper

    #region ValueObjectOrError

    public sealed class ValueObjectOrError<T>
    {
        private readonly bool _isSuccess;

        private readonly T? _valueObject;

        private readonly Validation? _error;

        public bool IsSuccess => _isSuccess;

        public Validation Error
        {
            get
            {
                if (!_isSuccess)
                {
                    return _error!;
                }

                return Validation.Ok;
            }
        }

        public T ValueObject
        {
            get
            {
                if (!_isSuccess)
                {
                    throw new InvalidOperationException("Cannot access the value object as it is not valid: " + _error?.ErrorMessage);
                }

                return _valueObject!;
            }
        }

        public ValueObjectOrError(T valueObject)
        {
            _isSuccess = true;
            _valueObject = valueObject;
            _error = Validation.Ok;
        }

        public ValueObjectOrError(Validation error)
        {
            _isSuccess = false;
            _valueObject = default(T);
            _error = error;
        }
    }

    #endregion //  ValueObjectOrError
}
#nullable restore
