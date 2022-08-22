using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Converters;
using System;

namespace SOE.Converters
{
    public class TimeSpanConverter : DocumentConverter
    {
        private static readonly DateTime BaseDateTime = new DateTime(1, 1, 1, 0, 0, 0);
        public TimeSpanConverter(Type targetType) : base(targetType)
        {
        }

        public override bool ConvertTo(object? value, out object? result)
        {
            if (value is TimeSpan timeSpan)
            {
                result = new Plugin.CloudFirestore.Timestamp(BaseDateTime.Add(timeSpan).ToLocalTime());
                return true;
            }
            result = null;
            return false;
        }

        public override bool ConvertFrom(DocumentObject value, out object? result)
        {
            Type? type = Nullable.GetUnderlyingType(TargetType) ?? TargetType;
            if (type != typeof(TimeSpan) || value.Type != DocumentObjectType.Timestapm)
            {
                result = null;
                return false;
            }
            TimeSpan timeSpan = value.Timestamp.ToDateTime() - BaseDateTime;
            result = timeSpan;
            return true;
        }
    }
}
