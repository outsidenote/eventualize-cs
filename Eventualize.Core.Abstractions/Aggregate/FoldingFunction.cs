using System.Data;
using Eventualize.Core;
namespace Eventualize.Core.AggregateType;
public delegate object FoldingFunction(object oldState, EventEntity SerializedEvent);

public delegate StateType FoldingFunction<StateType>(StateType oldState, EventEntity SerializedEvent);

public interface IFoldingFunction<StateType>
{
    public StateType Fold(StateType oldState, EventEntity serializedEvent);

}