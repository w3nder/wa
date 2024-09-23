// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Schema.JsonSchemaModel
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Schema
{
  [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
  internal class JsonSchemaModel
  {
    public bool Required { get; set; }

    public JsonSchemaType Type { get; set; }

    public int? MinimumLength { get; set; }

    public int? MaximumLength { get; set; }

    public double? DivisibleBy { get; set; }

    public double? Minimum { get; set; }

    public double? Maximum { get; set; }

    public bool ExclusiveMinimum { get; set; }

    public bool ExclusiveMaximum { get; set; }

    public int? MinimumItems { get; set; }

    public int? MaximumItems { get; set; }

    public IList<string> Patterns { get; set; }

    public IList<JsonSchemaModel> Items { get; set; }

    public IDictionary<string, JsonSchemaModel> Properties { get; set; }

    public IDictionary<string, JsonSchemaModel> PatternProperties { get; set; }

    public JsonSchemaModel AdditionalProperties { get; set; }

    public JsonSchemaModel AdditionalItems { get; set; }

    public bool PositionalItemsValidation { get; set; }

    public bool AllowAdditionalProperties { get; set; }

    public bool AllowAdditionalItems { get; set; }

    public bool UniqueItems { get; set; }

    public IList<JToken> Enum { get; set; }

    public JsonSchemaType Disallow { get; set; }

    public JsonSchemaModel()
    {
      this.Type = JsonSchemaType.Any;
      this.AllowAdditionalProperties = true;
      this.AllowAdditionalItems = true;
      this.Required = false;
    }

    public static JsonSchemaModel Create(IList<JsonSchema> schemata)
    {
      JsonSchemaModel model = new JsonSchemaModel();
      foreach (JsonSchema schema in (IEnumerable<JsonSchema>) schemata)
        JsonSchemaModel.Combine(model, schema);
      return model;
    }

    private static void Combine(JsonSchemaModel model, JsonSchema schema)
    {
      model.Required = model.Required || ((int) schema.Required ?? 0) != 0;
      model.Type &= (JsonSchemaType) ((int) schema.Type ?? (int) sbyte.MaxValue);
      model.MinimumLength = MathUtils.Max(model.MinimumLength, schema.MinimumLength);
      model.MaximumLength = MathUtils.Min(model.MaximumLength, schema.MaximumLength);
      model.DivisibleBy = MathUtils.Max(model.DivisibleBy, schema.DivisibleBy);
      model.Minimum = MathUtils.Max(model.Minimum, schema.Minimum);
      model.Maximum = MathUtils.Max(model.Maximum, schema.Maximum);
      model.ExclusiveMinimum = model.ExclusiveMinimum || ((int) schema.ExclusiveMinimum ?? 0) != 0;
      model.ExclusiveMaximum = model.ExclusiveMaximum || ((int) schema.ExclusiveMaximum ?? 0) != 0;
      model.MinimumItems = MathUtils.Max(model.MinimumItems, schema.MinimumItems);
      model.MaximumItems = MathUtils.Min(model.MaximumItems, schema.MaximumItems);
      model.PositionalItemsValidation = model.PositionalItemsValidation || schema.PositionalItemsValidation;
      model.AllowAdditionalProperties = model.AllowAdditionalProperties && schema.AllowAdditionalProperties;
      model.AllowAdditionalItems = model.AllowAdditionalItems && schema.AllowAdditionalItems;
      model.UniqueItems = model.UniqueItems || schema.UniqueItems;
      if (schema.Enum != null)
      {
        if (model.Enum == null)
          model.Enum = (IList<JToken>) new List<JToken>();
        model.Enum.AddRangeDistinct<JToken>((IEnumerable<JToken>) schema.Enum, (IEqualityComparer<JToken>) JToken.EqualityComparer);
      }
      model.Disallow |= (JsonSchemaType) ((int) schema.Disallow ?? 0);
      if (schema.Pattern == null)
        return;
      if (model.Patterns == null)
        model.Patterns = (IList<string>) new List<string>();
      model.Patterns.AddDistinct<string>(schema.Pattern);
    }
  }
}
