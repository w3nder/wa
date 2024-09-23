// Decompiled with JetBrains decompiler
// Type: WhatsApp.UserStatusProperties
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace WhatsApp
{
  public class UserStatusProperties
  {
    private UserStatus UserStatus;

    public UserStatusProperties.BusinessUserProperties BusinessUserPropertiesField { get; set; }

    public bool? HasSentHsm { get; set; }

    public static UserStatusProperties Deserialize(Stream stream)
    {
      UserStatusProperties instance = new UserStatusProperties();
      UserStatusProperties.Deserialize(stream, instance);
      return instance;
    }

    public static UserStatusProperties DeserializeLengthDelimited(Stream stream)
    {
      UserStatusProperties instance = new UserStatusProperties();
      UserStatusProperties.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static UserStatusProperties DeserializeLength(Stream stream, int length)
    {
      UserStatusProperties instance = new UserStatusProperties();
      UserStatusProperties.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static UserStatusProperties Deserialize(byte[] buffer)
    {
      UserStatusProperties instance = new UserStatusProperties();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        UserStatusProperties.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static UserStatusProperties Deserialize(byte[] buffer, UserStatusProperties instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        UserStatusProperties.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static UserStatusProperties Deserialize(Stream stream, UserStatusProperties instance)
    {
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_8;
          case 10:
            if (instance.BusinessUserPropertiesField == null)
            {
              instance.BusinessUserPropertiesField = UserStatusProperties.BusinessUserProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            UserStatusProperties.BusinessUserProperties.DeserializeLengthDelimited(stream, instance.BusinessUserPropertiesField);
            continue;
          case 16:
            instance.HasSentHsm = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            if (key.Field != 0U)
            {
              ProtocolParser.SkipKey(stream, key);
              continue;
            }
            goto label_6;
        }
      }
label_6:
      throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_8:
      return instance;
    }

    public static UserStatusProperties DeserializeLengthDelimited(
      Stream stream,
      UserStatusProperties instance)
    {
      long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 10:
            if (instance.BusinessUserPropertiesField == null)
            {
              instance.BusinessUserPropertiesField = UserStatusProperties.BusinessUserProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            UserStatusProperties.BusinessUserProperties.DeserializeLengthDelimited(stream, instance.BusinessUserPropertiesField);
            continue;
          case 16:
            instance.HasSentHsm = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            if (key.Field == 0U)
              throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
            ProtocolParser.SkipKey(stream, key);
            continue;
        }
      }
      if (stream.Position != num)
        throw new ProtocolBufferException("Read past max limit");
      return instance;
    }

    public static UserStatusProperties DeserializeLength(
      Stream stream,
      int length,
      UserStatusProperties instance)
    {
      long num = stream.Position + (long) length;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 10:
            if (instance.BusinessUserPropertiesField == null)
            {
              instance.BusinessUserPropertiesField = UserStatusProperties.BusinessUserProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            UserStatusProperties.BusinessUserProperties.DeserializeLengthDelimited(stream, instance.BusinessUserPropertiesField);
            continue;
          case 16:
            instance.HasSentHsm = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            if (key.Field == 0U)
              throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
            ProtocolParser.SkipKey(stream, key);
            continue;
        }
      }
      if (stream.Position != num)
        throw new ProtocolBufferException("Read past max limit");
      return instance;
    }

    public static void Serialize(Stream stream, UserStatusProperties instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.BusinessUserPropertiesField != null)
      {
        stream.WriteByte((byte) 10);
        stream1.SetLength(0L);
        UserStatusProperties.BusinessUserProperties.Serialize((Stream) stream1, instance.BusinessUserPropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.HasSentHsm.HasValue)
      {
        stream.WriteByte((byte) 16);
        ProtocolParser.WriteBool(stream, instance.HasSentHsm.Value);
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(UserStatusProperties instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        UserStatusProperties.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, UserStatusProperties instance)
    {
      byte[] bytes = UserStatusProperties.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public static bool HasUserStatusProperties(UserStatus userStatus)
    {
      return userStatus.InternalProperties != null;
    }

    public static UserStatusProperties GetForUserStatus(UserStatus userStatus)
    {
      if (userStatus == null)
        return (UserStatusProperties) null;
      UserStatusProperties forUserStatus = userStatus.InternalProperties ?? new UserStatusProperties();
      forUserStatus.UserStatus = userStatus;
      return forUserStatus;
    }

    public void Save()
    {
      if (this.UserStatus == null)
        throw new ArgumentNullException("No user Status associated with these properties");
      this.UserStatus.InternalProperties = this;
    }

    public void SetForUserStatus(UserStatus userStatus)
    {
      this.UserStatus = userStatus.InternalProperties == null ? userStatus : throw new ArgumentNullException("Overriding UserStatusProperties in Conversation, Jid: " + userStatus.Jid);
      userStatus.InternalProperties = this;
    }

    public UserStatusProperties.BusinessUserProperties EnsureBusinessUserProperties
    {
      get
      {
        if (this.BusinessUserPropertiesField == null)
          this.BusinessUserPropertiesField = new UserStatusProperties.BusinessUserProperties();
        return this.BusinessUserPropertiesField;
      }
    }

    public bool UpdateBusinessUserProperties(BizProfileDetails newProfileDetails)
    {
      UserStatusProperties.BusinessUserProperties businessUserProperties = this.EnsureBusinessUserProperties;
      if (businessUserProperties.Tag == newProfileDetails?.Tag)
        return false;
      businessUserProperties.Tag = newProfileDetails?.Tag;
      businessUserProperties.Address = newProfileDetails?.Address;
      businessUserProperties.Description = newProfileDetails?.Description;
      businessUserProperties.Email = newProfileDetails?.Email;
      businessUserProperties.VerticalDescription = newProfileDetails?.VerticalDescription;
      businessUserProperties.VerticalCanonical = newProfileDetails?.VerticalCanonical;
      businessUserProperties.Websites = newProfileDetails != null ? newProfileDetails.Websites.Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))).ToList<string>() : (List<string>) null;
      businessUserProperties.Latitude = (double?) newProfileDetails?.Latitude;
      businessUserProperties.Longitude = (double?) newProfileDetails?.Longitude;
      businessUserProperties.BizHoursBlob = newProfileDetails?.Hours?.Serialize();
      return true;
    }

    public class BusinessUserProperties
    {
      public string Tag { get; set; }

      public string Address { get; set; }

      public string Description { get; set; }

      public string VerticalDescription { get; set; }

      public string VerticalCanonical { get; set; }

      public string Email { get; set; }

      public string WebsitesObsolete { get; set; }

      public List<string> Websites { get; set; }

      [Obsolete]
      public bool? VerifiedForUi { get; set; }

      [Obsolete]
      public UserStatusProperties.BusinessUserProperties.SysMsgVerificationState? SysMsgVerifyState { get; set; }

      public double? Latitude { get; set; }

      public double? Longitude { get; set; }

      public byte[] BizHoursBlob { get; set; }

      public int? LastDisplayedVerifiedLevel { get; set; }

      public string LastDisplayedVerifiedName { get; set; }

      public int? LastDisplayedTier { get; set; }

      public static UserStatusProperties.BusinessUserProperties Deserialize(Stream stream)
      {
        UserStatusProperties.BusinessUserProperties instance = new UserStatusProperties.BusinessUserProperties();
        UserStatusProperties.BusinessUserProperties.Deserialize(stream, instance);
        return instance;
      }

      public static UserStatusProperties.BusinessUserProperties DeserializeLengthDelimited(
        Stream stream)
      {
        UserStatusProperties.BusinessUserProperties instance = new UserStatusProperties.BusinessUserProperties();
        UserStatusProperties.BusinessUserProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static UserStatusProperties.BusinessUserProperties DeserializeLength(
        Stream stream,
        int length)
      {
        UserStatusProperties.BusinessUserProperties instance = new UserStatusProperties.BusinessUserProperties();
        UserStatusProperties.BusinessUserProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static UserStatusProperties.BusinessUserProperties Deserialize(byte[] buffer)
      {
        UserStatusProperties.BusinessUserProperties instance = new UserStatusProperties.BusinessUserProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          UserStatusProperties.BusinessUserProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static UserStatusProperties.BusinessUserProperties Deserialize(
        byte[] buffer,
        UserStatusProperties.BusinessUserProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          UserStatusProperties.BusinessUserProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static UserStatusProperties.BusinessUserProperties Deserialize(
        Stream stream,
        UserStatusProperties.BusinessUserProperties instance)
      {
        BinaryReader binaryReader = new BinaryReader(stream);
        if (instance.Websites == null)
          instance.Websites = new List<string>();
        while (true)
        {
          SilentOrbit.ProtocolBuffers.Key key;
          do
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                goto label_23;
              case 10:
                instance.Tag = ProtocolParser.ReadString(stream);
                continue;
              case 18:
                instance.Address = ProtocolParser.ReadString(stream);
                continue;
              case 26:
                instance.Description = ProtocolParser.ReadString(stream);
                continue;
              case 34:
                instance.VerticalDescription = ProtocolParser.ReadString(stream);
                continue;
              case 42:
                instance.VerticalCanonical = ProtocolParser.ReadString(stream);
                continue;
              case 50:
                instance.Email = ProtocolParser.ReadString(stream);
                continue;
              case 58:
                instance.WebsitesObsolete = ProtocolParser.ReadString(stream);
                continue;
              case 66:
                instance.Websites.Add(ProtocolParser.ReadString(stream));
                continue;
              case 72:
                instance.VerifiedForUi = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 80:
                instance.SysMsgVerifyState = new UserStatusProperties.BusinessUserProperties.SysMsgVerificationState?((UserStatusProperties.BusinessUserProperties.SysMsgVerificationState) ProtocolParser.ReadUInt64(stream));
                continue;
              case 89:
                instance.Latitude = new double?(binaryReader.ReadDouble());
                continue;
              case 97:
                instance.Longitude = new double?(binaryReader.ReadDouble());
                continue;
              case 106:
                instance.BizHoursBlob = ProtocolParser.ReadBytes(stream);
                continue;
              case 112:
                instance.LastDisplayedVerifiedLevel = new int?((int) ProtocolParser.ReadUInt64(stream));
                continue;
              case 122:
                instance.LastDisplayedVerifiedName = ProtocolParser.ReadString(stream);
                continue;
              default:
                key = ProtocolParser.ReadKey((byte) firstByte, stream);
                switch (key.Field)
                {
                  case 0:
                    throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                  case 16:
                    continue;
                  default:
                    goto label_22;
                }
            }
          }
          while (key.WireType != Wire.Varint);
          instance.LastDisplayedTier = new int?((int) ProtocolParser.ReadUInt64(stream));
          continue;
label_22:
          ProtocolParser.SkipKey(stream, key);
        }
label_23:
        return instance;
      }

      public static UserStatusProperties.BusinessUserProperties DeserializeLengthDelimited(
        Stream stream,
        UserStatusProperties.BusinessUserProperties instance)
      {
        BinaryReader binaryReader = new BinaryReader(stream);
        if (instance.Websites == null)
          instance.Websites = new List<string>();
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.Tag = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Address = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Description = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.VerticalDescription = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.VerticalCanonical = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.Email = ProtocolParser.ReadString(stream);
              continue;
            case 58:
              instance.WebsitesObsolete = ProtocolParser.ReadString(stream);
              continue;
            case 66:
              instance.Websites.Add(ProtocolParser.ReadString(stream));
              continue;
            case 72:
              instance.VerifiedForUi = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 80:
              instance.SysMsgVerifyState = new UserStatusProperties.BusinessUserProperties.SysMsgVerificationState?((UserStatusProperties.BusinessUserProperties.SysMsgVerificationState) ProtocolParser.ReadUInt64(stream));
              continue;
            case 89:
              instance.Latitude = new double?(binaryReader.ReadDouble());
              continue;
            case 97:
              instance.Longitude = new double?(binaryReader.ReadDouble());
              continue;
            case 106:
              instance.BizHoursBlob = ProtocolParser.ReadBytes(stream);
              continue;
            case 112:
              instance.LastDisplayedVerifiedLevel = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 122:
              instance.LastDisplayedVerifiedName = ProtocolParser.ReadString(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.Varint)
                  {
                    instance.LastDisplayedTier = new int?((int) ProtocolParser.ReadUInt64(stream));
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

      public static UserStatusProperties.BusinessUserProperties DeserializeLength(
        Stream stream,
        int length,
        UserStatusProperties.BusinessUserProperties instance)
      {
        BinaryReader binaryReader = new BinaryReader(stream);
        if (instance.Websites == null)
          instance.Websites = new List<string>();
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.Tag = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Address = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Description = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.VerticalDescription = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.VerticalCanonical = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.Email = ProtocolParser.ReadString(stream);
              continue;
            case 58:
              instance.WebsitesObsolete = ProtocolParser.ReadString(stream);
              continue;
            case 66:
              instance.Websites.Add(ProtocolParser.ReadString(stream));
              continue;
            case 72:
              instance.VerifiedForUi = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 80:
              instance.SysMsgVerifyState = new UserStatusProperties.BusinessUserProperties.SysMsgVerificationState?((UserStatusProperties.BusinessUserProperties.SysMsgVerificationState) ProtocolParser.ReadUInt64(stream));
              continue;
            case 89:
              instance.Latitude = new double?(binaryReader.ReadDouble());
              continue;
            case 97:
              instance.Longitude = new double?(binaryReader.ReadDouble());
              continue;
            case 106:
              instance.BizHoursBlob = ProtocolParser.ReadBytes(stream);
              continue;
            case 112:
              instance.LastDisplayedVerifiedLevel = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 122:
              instance.LastDisplayedVerifiedName = ProtocolParser.ReadString(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.Varint)
                  {
                    instance.LastDisplayedTier = new int?((int) ProtocolParser.ReadUInt64(stream));
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

      public static void Serialize(
        Stream stream,
        UserStatusProperties.BusinessUserProperties instance)
      {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Tag != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Tag));
        }
        if (instance.Address != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Address));
        }
        if (instance.Description != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Description));
        }
        if (instance.VerticalDescription != null)
        {
          stream.WriteByte((byte) 34);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.VerticalDescription));
        }
        if (instance.VerticalCanonical != null)
        {
          stream.WriteByte((byte) 42);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.VerticalCanonical));
        }
        if (instance.Email != null)
        {
          stream.WriteByte((byte) 50);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Email));
        }
        if (instance.WebsitesObsolete != null)
        {
          stream.WriteByte((byte) 58);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.WebsitesObsolete));
        }
        if (instance.Websites != null)
        {
          foreach (string website in instance.Websites)
          {
            stream.WriteByte((byte) 66);
            ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(website));
          }
        }
        if (instance.VerifiedForUi.HasValue)
        {
          stream.WriteByte((byte) 72);
          ProtocolParser.WriteBool(stream, instance.VerifiedForUi.Value);
        }
        if (instance.SysMsgVerifyState.HasValue)
        {
          stream.WriteByte((byte) 80);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.SysMsgVerifyState.Value);
        }
        if (instance.Latitude.HasValue)
        {
          stream.WriteByte((byte) 89);
          binaryWriter.Write(instance.Latitude.Value);
        }
        if (instance.Longitude.HasValue)
        {
          stream.WriteByte((byte) 97);
          binaryWriter.Write(instance.Longitude.Value);
        }
        if (instance.BizHoursBlob != null)
        {
          stream.WriteByte((byte) 106);
          ProtocolParser.WriteBytes(stream, instance.BizHoursBlob);
        }
        if (instance.LastDisplayedVerifiedLevel.HasValue)
        {
          stream.WriteByte((byte) 112);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.LastDisplayedVerifiedLevel.Value);
        }
        if (instance.LastDisplayedVerifiedName != null)
        {
          stream.WriteByte((byte) 122);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.LastDisplayedVerifiedName));
        }
        if (instance.LastDisplayedTier.HasValue)
        {
          stream.WriteByte((byte) 128);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.LastDisplayedTier.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(
        UserStatusProperties.BusinessUserProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          UserStatusProperties.BusinessUserProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        UserStatusProperties.BusinessUserProperties instance)
      {
        byte[] bytes = UserStatusProperties.BusinessUserProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public enum SysMsgVerificationState
      {
        UNKNOWN,
        STANDARD,
        VERIFIED,
        VERIFIEDHIGH,
      }
    }
  }
}
