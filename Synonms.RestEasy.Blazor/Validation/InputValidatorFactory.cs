namespace Synonms.RestEasy.Blazor.Validation;

public static class InputValidatorFactory
{
    private static readonly List<IInputValidator> Validators = new()
    {
        new DecimalInputValidator(),
        new StringInputValidator()
    };

    public static IInputValidator? Create(string dataType) =>
        Validators.SingleOrDefault(x => x.DataType == dataType);
}