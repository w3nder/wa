// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.WebFeatures
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.IO;


namespace WhatsApp.ProtoBuf
{
  public class WebFeatures
  {
    public WebFeatures.Flag? LabelsDisplay { get; set; }

    public WebFeatures.Flag? VoipIndividualOutgoing { get; set; }

    public WebFeatures.Flag? GroupsV3 { get; set; }

    public WebFeatures.Flag? GroupsV3Create { get; set; }

    public WebFeatures.Flag? ChangeNumberV2 { get; set; }

    public WebFeatures.Flag? QueryStatusV3Thumbnail { get; set; }

    public WebFeatures.Flag? LiveLocations { get; set; }

    public WebFeatures.Flag? QueryVname { get; set; }

    public WebFeatures.Flag? VoipIndividualIncoming { get; set; }

    public WebFeatures.Flag? QuickRepliesQuery { get; set; }

    public WebFeatures.Flag? Payments { get; set; }

    public WebFeatures.Flag? StickerPackQuery { get; set; }

    public WebFeatures.Flag? LiveLocationsFinal { get; set; }

    public WebFeatures.Flag? LabelsEdit { get; set; }

    public WebFeatures.Flag? MediaUpload { get; set; }

    public WebFeatures.Flag? MediaUploadRichQuickReplies { get; set; }

    public WebFeatures.Flag? VnameV2 { get; set; }

    public WebFeatures.Flag? VideoPlaybackUrl { get; set; }

    public WebFeatures.Flag? StatusRanking { get; set; }

    public WebFeatures.Flag? VoipIndividualVideo { get; set; }

    public WebFeatures.Flag? ThirdPartyStickers { get; set; }

    public static WebFeatures Deserialize(Stream stream)
    {
      WebFeatures instance = new WebFeatures();
      WebFeatures.Deserialize(stream, instance);
      return instance;
    }

    public static WebFeatures DeserializeLengthDelimited(Stream stream)
    {
      WebFeatures instance = new WebFeatures();
      WebFeatures.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static WebFeatures DeserializeLength(Stream stream, int length)
    {
      WebFeatures instance = new WebFeatures();
      WebFeatures.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static WebFeatures Deserialize(byte[] buffer)
    {
      WebFeatures instance = new WebFeatures();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        WebFeatures.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static WebFeatures Deserialize(byte[] buffer, WebFeatures instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        WebFeatures.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static WebFeatures Deserialize(Stream stream, WebFeatures instance)
    {
      while (true)
      {
        SilentOrbit.ProtocolBuffers.Key key;
        do
        {
          do
          {
            do
            {
              do
              {
                do
                {
                  do
                  {
                    int firstByte = stream.ReadByte();
                    switch (firstByte)
                    {
                      case -1:
                        goto label_31;
                      case 8:
                        instance.LabelsDisplay = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 16:
                        instance.VoipIndividualOutgoing = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 24:
                        instance.GroupsV3 = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 32:
                        instance.GroupsV3Create = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 40:
                        instance.ChangeNumberV2 = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 48:
                        instance.QueryStatusV3Thumbnail = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 56:
                        instance.LiveLocations = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 64:
                        instance.QueryVname = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 72:
                        instance.VoipIndividualIncoming = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 80:
                        instance.QuickRepliesQuery = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 88:
                        instance.Payments = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 96:
                        instance.StickerPackQuery = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 104:
                        instance.LiveLocationsFinal = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 112:
                        instance.LabelsEdit = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      case 120:
                        instance.MediaUpload = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                        continue;
                      default:
                        key = ProtocolParser.ReadKey((byte) firstByte, stream);
                        switch (key.Field)
                        {
                          case 0:
                            throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                          case 18:
                            continue;
                          case 19:
                            goto label_20;
                          case 20:
                            goto label_22;
                          case 21:
                            goto label_24;
                          case 22:
                            goto label_26;
                          case 23:
                            goto label_28;
                          default:
                            goto label_30;
                        }
                    }
                  }
                  while (key.WireType != Wire.Varint);
                  instance.MediaUploadRichQuickReplies = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
label_20:;
                }
                while (key.WireType != Wire.Varint);
                instance.VnameV2 = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                continue;
label_22:;
              }
              while (key.WireType != Wire.Varint);
              instance.VideoPlaybackUrl = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
              continue;
label_24:;
            }
            while (key.WireType != Wire.Varint);
            instance.StatusRanking = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
label_26:;
          }
          while (key.WireType != Wire.Varint);
          instance.VoipIndividualVideo = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
          continue;
label_28:;
        }
        while (key.WireType != Wire.Varint);
        instance.ThirdPartyStickers = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
        continue;
label_30:
        ProtocolParser.SkipKey(stream, key);
      }
label_31:
      return instance;
    }

    public static WebFeatures DeserializeLengthDelimited(Stream stream, WebFeatures instance)
    {
      long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 8:
            instance.LabelsDisplay = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 16:
            instance.VoipIndividualOutgoing = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 24:
            instance.GroupsV3 = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 32:
            instance.GroupsV3Create = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 40:
            instance.ChangeNumberV2 = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 48:
            instance.QueryStatusV3Thumbnail = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 56:
            instance.LiveLocations = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 64:
            instance.QueryVname = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 72:
            instance.VoipIndividualIncoming = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 80:
            instance.QuickRepliesQuery = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 88:
            instance.Payments = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 96:
            instance.StickerPackQuery = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 104:
            instance.LiveLocationsFinal = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 112:
            instance.LabelsEdit = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 120:
            instance.MediaUpload = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            switch (key.Field)
            {
              case 0:
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              case 18:
                if (key.WireType == Wire.Varint)
                {
                  instance.MediaUploadRichQuickReplies = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 19:
                if (key.WireType == Wire.Varint)
                {
                  instance.VnameV2 = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 20:
                if (key.WireType == Wire.Varint)
                {
                  instance.VideoPlaybackUrl = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 21:
                if (key.WireType == Wire.Varint)
                {
                  instance.StatusRanking = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 22:
                if (key.WireType == Wire.Varint)
                {
                  instance.VoipIndividualVideo = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 23:
                if (key.WireType == Wire.Varint)
                {
                  instance.ThirdPartyStickers = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              default:
                ProtocolParser.SkipKey(stream, key);
                continue;
            }
        }
      }
      if (stream.Position != num)
        throw new ProtocolBufferException("Read past max limit");
      return instance;
    }

    public static WebFeatures DeserializeLength(Stream stream, int length, WebFeatures instance)
    {
      long num = stream.Position + (long) length;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 8:
            instance.LabelsDisplay = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 16:
            instance.VoipIndividualOutgoing = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 24:
            instance.GroupsV3 = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 32:
            instance.GroupsV3Create = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 40:
            instance.ChangeNumberV2 = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 48:
            instance.QueryStatusV3Thumbnail = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 56:
            instance.LiveLocations = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 64:
            instance.QueryVname = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 72:
            instance.VoipIndividualIncoming = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 80:
            instance.QuickRepliesQuery = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 88:
            instance.Payments = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 96:
            instance.StickerPackQuery = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 104:
            instance.LiveLocationsFinal = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 112:
            instance.LabelsEdit = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          case 120:
            instance.MediaUpload = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            switch (key.Field)
            {
              case 0:
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              case 18:
                if (key.WireType == Wire.Varint)
                {
                  instance.MediaUploadRichQuickReplies = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 19:
                if (key.WireType == Wire.Varint)
                {
                  instance.VnameV2 = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 20:
                if (key.WireType == Wire.Varint)
                {
                  instance.VideoPlaybackUrl = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 21:
                if (key.WireType == Wire.Varint)
                {
                  instance.StatusRanking = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 22:
                if (key.WireType == Wire.Varint)
                {
                  instance.VoipIndividualVideo = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 23:
                if (key.WireType == Wire.Varint)
                {
                  instance.ThirdPartyStickers = new WebFeatures.Flag?((WebFeatures.Flag) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              default:
                ProtocolParser.SkipKey(stream, key);
                continue;
            }
        }
      }
      if (stream.Position != num)
        throw new ProtocolBufferException("Read past max limit");
      return instance;
    }

    public static void Serialize(Stream stream, WebFeatures instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.LabelsDisplay.HasValue)
      {
        stream.WriteByte((byte) 8);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.LabelsDisplay.Value);
      }
      if (instance.VoipIndividualOutgoing.HasValue)
      {
        stream.WriteByte((byte) 16);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.VoipIndividualOutgoing.Value);
      }
      if (instance.GroupsV3.HasValue)
      {
        stream.WriteByte((byte) 24);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.GroupsV3.Value);
      }
      if (instance.GroupsV3Create.HasValue)
      {
        stream.WriteByte((byte) 32);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.GroupsV3Create.Value);
      }
      if (instance.ChangeNumberV2.HasValue)
      {
        stream.WriteByte((byte) 40);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.ChangeNumberV2.Value);
      }
      if (instance.QueryStatusV3Thumbnail.HasValue)
      {
        stream.WriteByte((byte) 48);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.QueryStatusV3Thumbnail.Value);
      }
      if (instance.LiveLocations.HasValue)
      {
        stream.WriteByte((byte) 56);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.LiveLocations.Value);
      }
      if (instance.QueryVname.HasValue)
      {
        stream.WriteByte((byte) 64);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.QueryVname.Value);
      }
      if (instance.VoipIndividualIncoming.HasValue)
      {
        stream.WriteByte((byte) 72);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.VoipIndividualIncoming.Value);
      }
      if (instance.QuickRepliesQuery.HasValue)
      {
        stream.WriteByte((byte) 80);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.QuickRepliesQuery.Value);
      }
      if (instance.Payments.HasValue)
      {
        stream.WriteByte((byte) 88);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.Payments.Value);
      }
      if (instance.StickerPackQuery.HasValue)
      {
        stream.WriteByte((byte) 96);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.StickerPackQuery.Value);
      }
      if (instance.LiveLocationsFinal.HasValue)
      {
        stream.WriteByte((byte) 104);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.LiveLocationsFinal.Value);
      }
      if (instance.LabelsEdit.HasValue)
      {
        stream.WriteByte((byte) 112);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.LabelsEdit.Value);
      }
      if (instance.MediaUpload.HasValue)
      {
        stream.WriteByte((byte) 120);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.MediaUpload.Value);
      }
      if (instance.MediaUploadRichQuickReplies.HasValue)
      {
        stream.WriteByte((byte) 144);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.MediaUploadRichQuickReplies.Value);
      }
      if (instance.VnameV2.HasValue)
      {
        stream.WriteByte((byte) 152);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.VnameV2.Value);
      }
      if (instance.VideoPlaybackUrl.HasValue)
      {
        stream.WriteByte((byte) 160);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.VideoPlaybackUrl.Value);
      }
      if (instance.StatusRanking.HasValue)
      {
        stream.WriteByte((byte) 168);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.StatusRanking.Value);
      }
      if (instance.VoipIndividualVideo.HasValue)
      {
        stream.WriteByte((byte) 176);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.VoipIndividualVideo.Value);
      }
      if (instance.ThirdPartyStickers.HasValue)
      {
        stream.WriteByte((byte) 184);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.ThirdPartyStickers.Value);
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(WebFeatures instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        WebFeatures.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, WebFeatures instance)
    {
      byte[] bytes = WebFeatures.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public enum Flag
    {
      NOT_IMPLEMENTED,
      IMPLEMENTED,
      OPTIONAL,
    }
  }
}
