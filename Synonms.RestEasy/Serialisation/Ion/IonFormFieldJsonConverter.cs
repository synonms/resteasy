using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Extensions;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonFormFieldJsonConverter : JsonConverter<FormField>
{
    public override FormField? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, FormField value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString(IonPropertyNames.FormFields.Name, value.Name);
        writer.WriteString(IonPropertyNames.FormFields.Type, value.Type ?? DataTypes.String);

        if (string.IsNullOrEmpty(value.ElementType) is false)
        {
            writer.WriteString(IonPropertyNames.FormFields.ElementType, value.ElementType);
        }

        if (value.ElementForm is not null)
        {
            writer.WritePropertyName(IonPropertyNames.FormFields.ElementForm);
            writer.WriteStartObject();
            writer.WritePropertyName(IonPropertyNames.Value);
            JsonSerializer.Serialize(writer, value.ElementForm, options);
            writer.WriteEndObject();
        }

        if (value.Form is not null)
        {
            writer.WritePropertyName(IonPropertyNames.FormFields.Form);
            writer.WriteStartObject();
            writer.WritePropertyName(IonPropertyNames.Value);
            JsonSerializer.Serialize(writer, value.Form, options);
            writer.WriteEndObject();
        }

        if (string.IsNullOrEmpty(value.Description) is false)
        {
            writer.WriteString(IonPropertyNames.FormFields.Description, value.Description);
        }

        if (value.IsEnabled is not null)
        {
            writer.WriteBoolean(IonPropertyNames.FormFields.Enabled, value.IsEnabled ?? true);
        }

        if (value.IsMutable is not null)
        {
            writer.WriteBoolean(IonPropertyNames.FormFields.Mutable, value.IsMutable ?? true);
        }

        if (value.IsRequired is not null)
        {
            writer.WriteBoolean(IonPropertyNames.FormFields.Required, value.IsRequired ?? false);
        }

        if (value.IsSecret is not null)
        {
            writer.WriteBoolean(IonPropertyNames.FormFields.Secret, value.IsSecret ?? false);
        }

        if (value.IsVisible is not null)
        {
            writer.WriteBoolean(IonPropertyNames.FormFields.Visible, value.IsVisible ?? true);
        }

        if (string.IsNullOrEmpty(value.Label) is false)
        {
            writer.WriteString(IonPropertyNames.FormFields.Label, value.Label);
        }

        if (value.Max is not null && value.Max.GetType().IsAssignableTo(value.ValueType))
        {
            writer.WritePropertyName(IonPropertyNames.FormFields.Max);
            JsonSerializer.Serialize(writer, value.Max, value.ValueType, options);
        }

        if (value.MaxLength is not null && value.MaxLength > 0)
        {
            writer.WriteNumber(IonPropertyNames.FormFields.MaxLength, value.MaxLength.Value);
        }

        if (value.MaxSize is not null && value.MaxSize > 0)
        {
            writer.WriteNumber(IonPropertyNames.FormFields.MaxSize, value.MaxSize.Value);
        }

        if (value.Min is not null && value.Min.GetType().IsAssignableTo(value.ValueType))
        {
            writer.WritePropertyName(IonPropertyNames.FormFields.Min);
            JsonSerializer.Serialize(writer, value.Min, value.ValueType, options);
        }

        if (value.MinLength is not null && value.MinLength > 0)
        {
            writer.WriteNumber(IonPropertyNames.FormFields.MinLength, value.MinLength.Value);
        }

        if (value.MinSize is not null && value.MinSize > 0)
        {
            writer.WriteNumber(IonPropertyNames.FormFields.MinSize, value.MinSize.Value);
        }

        if (string.IsNullOrEmpty(value.Pattern) is false)
        {
            writer.WriteString(IonPropertyNames.FormFields.Pattern, value.Pattern);
        }

        if (string.IsNullOrEmpty(value.Placeholder) is false)
        {
            writer.WriteString(IonPropertyNames.FormFields.Placeholder, value.Placeholder);
        }

        if (value.Value is not null && value.Value.GetType().IsAssignableTo(value.ValueType))
        {
            writer.WritePropertyName(IonPropertyNames.FormFields.Value);
            JsonSerializer.Serialize(writer, value.Value, value.ValueType, options);
        }

        if (value.Options is not null)
        {
            writer.WritePropertyName(IonPropertyNames.FormFields.Options);
            writer.WriteStartObject();
            writer.WritePropertyName(IonPropertyNames.Value);
            JsonSerializer.Serialize(writer, value.Options, options);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }

    private static IEnumerable<FormField>? TryGetForm(JsonElement rootElement, string formName, JsonSerializerOptions options)
    {
        if (rootElement.TryGetProperty(formName, out JsonElement formElement))
        {
            if (formElement.TryGetProperty(IonPropertyNames.Value, out JsonElement valueElement))
            {
                return JsonSerializer.Deserialize<IEnumerable<FormField>>(valueElement.ToString(), options);
            }
        }

        return null; 
    }

    private static IEnumerable<FormFieldOption>? TryGetOptions(JsonElement rootElement, Type valueType)
    {
        if (rootElement.TryGetProperty(IonPropertyNames.FormFields.Options, out JsonElement formFieldOptionsElement))
        {
            List<FormFieldOption> formFieldOptions = new();
            
            foreach (JsonElement formFieldOptionElement in formFieldOptionsElement.GetProperty(IonPropertyNames.Value).EnumerateArray())
            {
                object? optionValue = formFieldOptionElement.GetOptionalValue(IonPropertyNames.Value, valueType);

                if (optionValue is null)
                {
                    throw new JsonException($"Unable to determine FormFieldOption value for type [{valueType}] in element '{formFieldOptionElement.ToString()}'.");
                }
                
                string? optionLabel = formFieldOptionElement.GetOptionalString(IonPropertyNames.FormFields.Label);
                bool? isOptionEnabled = formFieldOptionElement.GetOptionalBool(IonPropertyNames.FormFields.Enabled);

                FormFieldOption formFieldOption = new(optionValue)
                {
                    Label = optionLabel,
                    IsEnabled = isOptionEnabled ?? true
                };
                
                formFieldOptions.Add(formFieldOption);
            }

            return formFieldOptions;
        }

        return null;
    }
}