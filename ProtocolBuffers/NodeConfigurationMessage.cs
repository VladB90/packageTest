// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: NodeConfigurationMessage.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;


namespace MBody
{

    /// <summary>Holder for reflection information generated from NodeConfigurationMessage.proto</summary>
    public static partial class NodeConfigurationMessageReflection
    {

        #region Descriptor
        /// <summary>File descriptor for NodeConfigurationMessage.proto</summary>
        public static pbr::FileDescriptor Descriptor
        {
            get { return descriptor; }
        }
        private static pbr::FileDescriptor descriptor;

        static NodeConfigurationMessageReflection()
        {
            byte[] descriptorData = global::System.Convert.FromBase64String(
                string.Concat(
                  "Ch5Ob2RlQ29uZmlndXJhdGlvbk1lc3NhZ2UucHJvdG8SBEFSVFAioAEKGE5v",
                  "ZGVDb25maWd1cmF0aW9uTWVzc2FnZRJMCg9jb25mZ3VyYXRpb25NYXAYASAD",
                  "KAsyMy5BUlRQLk5vZGVDb25maWd1cmF0aW9uTWVzc2FnZS5Db25mZ3VyYXRp",
                  "b25NYXBFbnRyeRo2ChRDb25mZ3VyYXRpb25NYXBFbnRyeRILCgNrZXkYASAB",
                  "KAkSDQoFdmFsdWUYAiABKAk6AjgBYgZwcm90bzM="));
            descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
                new pbr::FileDescriptor[] { },
                new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::MBody.NodeConfigurationMessage), global::MBody.NodeConfigurationMessage.Parser, new[]{ "ConfgurationMap" }, null, null, null, new pbr::GeneratedClrTypeInfo[] { null, })
                }));
        }
        #endregion

    }
    #region Messages
    public sealed partial class NodeConfigurationMessage : pb::IMessage<NodeConfigurationMessage>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        , pb::IBufferMessage
#endif
    {
        private static readonly pb::MessageParser<NodeConfigurationMessage> _parser = new pb::MessageParser<NodeConfigurationMessage>(() => new NodeConfigurationMessage());
        private pb::UnknownFieldSet _unknownFields;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public static pb::MessageParser<NodeConfigurationMessage> Parser { get { return _parser; } }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public static pbr::MessageDescriptor Descriptor
        {
            get { return global::MBody.NodeConfigurationMessageReflection.Descriptor.MessageTypes[0]; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        pbr::MessageDescriptor pb::IMessage.Descriptor
        {
            get { return Descriptor; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public NodeConfigurationMessage()
        {
            OnConstruction();
        }

        partial void OnConstruction();

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public NodeConfigurationMessage(NodeConfigurationMessage other) : this()
        {
            confgurationMap_ = other.confgurationMap_.Clone();
            _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public NodeConfigurationMessage Clone()
        {
            return new NodeConfigurationMessage(this);
        }

        /// <summary>Field number for the "confgurationMap" field.</summary>
        public const int ConfgurationMapFieldNumber = 1;
        private static readonly pbc::MapField<string, string>.Codec _map_confgurationMap_codec
            = new pbc::MapField<string, string>.Codec(pb::FieldCodec.ForString(10, ""), pb::FieldCodec.ForString(18, ""), 10);
        private readonly pbc::MapField<string, string> confgurationMap_ = new pbc::MapField<string, string>();
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public pbc::MapField<string, string> ConfgurationMap
        {
            get { return confgurationMap_; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public override bool Equals(object other)
        {
            return Equals(other as NodeConfigurationMessage);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public bool Equals(NodeConfigurationMessage other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            if (!ConfgurationMap.Equals(other.ConfgurationMap)) return false;
            return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public override int GetHashCode()
        {
            int hash = 1;
            hash ^= ConfgurationMap.GetHashCode();
            if (_unknownFields != null)
            {
                hash ^= _unknownFields.GetHashCode();
            }
            return hash;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public override string ToString()
        {
            return pb::JsonFormatter.ToDiagnosticString(this);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public void WriteTo(pb::CodedOutputStream output)
        {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
            output.WriteRawMessage(this);
#else
      confgurationMap_.WriteTo(output, _map_confgurationMap_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
#endif
        }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output)
        {
            confgurationMap_.WriteTo(ref output, _map_confgurationMap_codec);
            if (_unknownFields != null)
            {
                _unknownFields.WriteTo(ref output);
            }
        }
#endif

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public int CalculateSize()
        {
            int size = 0;
            size += confgurationMap_.CalculateSize(_map_confgurationMap_codec);
            if (_unknownFields != null)
            {
                size += _unknownFields.CalculateSize();
            }
            return size;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public void MergeFrom(NodeConfigurationMessage other)
        {
            if (other == null)
            {
                return;
            }
            confgurationMap_.Add(other.confgurationMap_);
            _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        public void MergeFrom(pb::CodedInputStream input)
        {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
            input.ReadRawMessage(this);
#else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            confgurationMap_.AddEntriesFrom(input, _map_confgurationMap_codec);
            break;
          }
        }
      }
#endif
        }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
        void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch (tag)
                {
                    default:
                        _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
                        break;
                    case 10:
                        {
                            confgurationMap_.AddEntriesFrom(ref input, _map_confgurationMap_codec);
                            break;
                        }
                }
            }
        }
#endif

    }

    #endregion

}

#endregion Designer generated code
