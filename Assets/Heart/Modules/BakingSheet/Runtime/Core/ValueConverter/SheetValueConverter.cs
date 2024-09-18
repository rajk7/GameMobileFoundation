﻿using System;

namespace Pancake.BakingSheet
{
    public abstract class SheetValueConverter<T> : ISheetValueConverter
    {
        bool ISheetValueConverter.CanConvert(Type type) { return typeof(T).IsAssignableFrom(type); }

        object ISheetValueConverter.StringToValue(Type type, string value, SheetValueConvertingContext context) { return StringToValue(type, value, context); }

        string ISheetValueConverter.ValueToString(Type type, object value, SheetValueConvertingContext context) { return ValueToString(type, (T) value, context); }

        protected abstract T StringToValue(Type type, string value, SheetValueConvertingContext context);
        protected abstract string ValueToString(Type type, T value, SheetValueConvertingContext context);
    }
}