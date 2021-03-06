﻿using JT808.Protocol.Extensions;
using JT808.Protocol.JT808Properties;
using JT808.Protocol.MessageBody;
using System;
using System.Collections.Generic;

namespace JT808.Protocol.JT808Formatters.MessageBodyFormatters
{
    public class JT808_0x8600Formatter : IJT808Formatter<JT808_0x8600>
    {
        public JT808_0x8600 Deserialize(ReadOnlySpan<byte> bytes, out int readSize)
        {
            int offset = 0;
            JT808_0x8600 jT808_0X8600 = new JT808_0x8600
            {
                SettingAreaProperty = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset),
                AreaCount = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset),
                AreaItems = new List<JT808CircleAreaProperty>()
            };
            for (var i = 0; i < jT808_0X8600.AreaCount; i++)
            {
                JT808CircleAreaProperty jT808CircleAreaProperty = new JT808CircleAreaProperty
                {
                    AreaId = JT808BinaryExtensions.ReadUInt32Little(bytes, ref offset),
                    AreaProperty = JT808BinaryExtensions.ReadUInt16Little(bytes, ref offset),
                    CenterPointLat = JT808BinaryExtensions.ReadUInt32Little(bytes, ref offset),
                    CenterPointLng = JT808BinaryExtensions.ReadUInt32Little(bytes, ref offset),
                    Radius = JT808BinaryExtensions.ReadUInt32Little(bytes, ref offset)
                };
                ReadOnlySpan<char> areaProperty16Bit = Convert.ToString(jT808CircleAreaProperty.AreaProperty, 2).PadLeft(16, '0').AsSpan();
                bool bit0Flag = areaProperty16Bit.Slice(areaProperty16Bit.Length - 1).ToString().Equals("0");
                if (!bit0Flag)
                {
                    jT808CircleAreaProperty.StartTime = JT808BinaryExtensions.ReadDateTime6Little(bytes, ref offset);
                    jT808CircleAreaProperty.EndTime = JT808BinaryExtensions.ReadDateTime6Little(bytes, ref offset);
                }
                bool bit1Flag = areaProperty16Bit.Slice(areaProperty16Bit.Length - 2, 1).ToString().Equals("0");
                if (!bit1Flag)
                {
                    jT808CircleAreaProperty.HighestSpeed = JT808BinaryExtensions.ReadUInt16Little(bytes, ref offset);
                    jT808CircleAreaProperty.OverspeedDuration = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                }
                jT808_0X8600.AreaItems.Add(jT808CircleAreaProperty);
            }
            readSize = offset;
            return jT808_0X8600;
        }

        public int Serialize(ref byte[] bytes, int offset, JT808_0x8600 value)
        {
            offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, value.SettingAreaProperty);
            if (value.AreaItems != null)
            {
                offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, (byte)value.AreaItems.Count);
                foreach (var item in value.AreaItems)
                {
                    offset += JT808BinaryExtensions.WriteUInt32Little(bytes, offset, item.AreaId);
                    offset += JT808BinaryExtensions.WriteUInt16Little(bytes, offset, item.AreaProperty);
                    offset += JT808BinaryExtensions.WriteUInt32Little(bytes, offset, item.CenterPointLat);
                    offset += JT808BinaryExtensions.WriteUInt32Little(bytes, offset, item.CenterPointLng);
                    offset += JT808BinaryExtensions.WriteUInt32Little(bytes, offset, item.Radius);
                    ReadOnlySpan<char> areaProperty16Bit = Convert.ToString(item.AreaProperty, 2).PadLeft(16, '0').AsSpan();
                    bool bit0Flag = areaProperty16Bit.Slice(areaProperty16Bit.Length - 1).ToString().Equals("0");
                    if (!bit0Flag)
                    {
                        if (item.StartTime.HasValue)
                        {
                            offset += JT808BinaryExtensions.WriteDateTime6Little(bytes, offset, item.StartTime.Value);
                        }
                        if (item.EndTime.HasValue)
                        {
                            offset += JT808BinaryExtensions.WriteDateTime6Little(bytes, offset, item.EndTime.Value);
                        }
                    }
                    bool bit1Flag = areaProperty16Bit.Slice(areaProperty16Bit.Length - 2, 1).ToString().Equals("0");
                    if (!bit1Flag)
                    {
                        if (item.HighestSpeed.HasValue)
                        {
                            offset += JT808BinaryExtensions.WriteUInt16Little(bytes, offset, item.HighestSpeed.Value);
                        }
                        if (item.OverspeedDuration.HasValue)
                        {
                            offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, item.OverspeedDuration.Value);
                        }
                    }
                }
            }
            return offset;
        }
    }
}
