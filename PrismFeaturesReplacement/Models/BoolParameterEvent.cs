namespace PrismFeaturesReplacement.Models;

public class BoolParameterEvent : ValueChangedMessage<bool>
{
    public BoolParameterEvent(bool value) : base(value)
    {
    }
}