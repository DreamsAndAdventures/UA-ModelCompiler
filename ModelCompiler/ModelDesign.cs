/* ========================================================================
 * Copyright (c) 2005-2016 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Opc.Ua.ModelCompiler
{
    public partial class ModelDesign
    {
        [XmlIgnore()]
        public Namespace TargetNamespaceInfo;

        [XmlIgnore()]
        public NamespaceTable NamespaceUris;
        
        [XmlIgnore()]
        public Dictionary<string,Export.ModelTableEntry> Dependencies;
    }

    public partial class LocalizedText
    {
        [XmlIgnore()]
        public bool IsAutogenerated;
    }

    public class HierarchyNode : IFormattable
    {
        public string RelativePath;
        public NodeDesign Instance;
        public List<NodeDesign> OverriddenNodes;
        public bool ExplicitlyDefined;
        public bool Inherited;
        public object Identifier;

        /// <summary>
        /// Returns the string representation of the object.
        /// </summary>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Returns the string representation of the object.
        /// </summary>
        public string ToString(string format, IFormatProvider provider)
        {
            if (format == null)
            {
                if (Instance != null && Instance.SymbolicId != null)
                {
                    return String.Format(provider, "{0}={1}", RelativePath, Instance.SymbolicId.Name);
                }

                return RelativePath;
            }

            throw new FormatException(Utils.Format("Invalid format string: '{0}'.", format));
        }
    }

    public class HierarchyReference : IFormattable
    {
        public string SourcePath;
        public XmlQualifiedName ReferenceType;
        public bool IsInverse;
        public string TargetPath;
        public XmlQualifiedName TargetId;
        public bool DefinedOnType;

        /// <summary>
        /// Returns the string representation of the object.
        /// </summary>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Returns the string representation of the object.
        /// </summary>
        public string ToString(string format, IFormatProvider provider)
        {
            if (format == null)
            {
                if (TargetId != null)
                {
                    return String.Format(provider, "{0} => {1}", SourcePath, TargetId.Name);
                }

                return String.Format(provider, "{0} => {1}", SourcePath, TargetPath);
            }

            throw new FormatException(Utils.Format("Invalid format string: '{0}'.", format));
        }
    }

    public class Hierarchy
    {
        public TypeDesign Type;
        public Dictionary<string, HierarchyNode> Nodes = new Dictionary<string, HierarchyNode>();
        public List<HierarchyNode> NodeList = new List<HierarchyNode>();
        public List<HierarchyReference> References = new List<HierarchyReference>();
    }

    /// <summary>
    /// A class that stores the model design for a Node.
    /// </summary>
    public partial class NodeDesign : IFormattable
    {
        [XmlIgnore()]
        public NodeDesign Parent;

        [XmlIgnore()]
        public bool HasChildren;

        [XmlIgnore()]
        public bool HasReferences;

        [XmlIgnore()]
        public NodeState State;

        [XmlIgnore()]
        public NodeState InstanceState;

        [XmlIgnore()]
        public Hierarchy Hierarchy;

        [XmlIgnore()]
        public string Handle;

        /// <summary>
        /// Returns the string representation of the object.
        /// </summary>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Returns the string representation of the object.
        /// </summary>
        public string ToString(string format, IFormatProvider provider)
        {
            if (format == null)
            {
                if (SymbolicName != null)
                {
                    return String.Format(provider, "{0}", SymbolicName.Name);
                }

                return String.Format(provider, "{0}", GetType().Name);
            }

            throw new FormatException(Utils.Format("Invalid format string: '{0}'.", format));
        }

        public static string CreateSymbolicId(XmlQualifiedName parentId, string childName)
        {
            if (parentId == null)
            {
                return childName;
            }

            return CreateSymbolicId(parentId.Name, childName);
        }

        public static string CreateSymbolicId(string parentId, string childName)
        {
            if (String.IsNullOrEmpty(childName))
            {
                return parentId;
            }

            if (String.IsNullOrEmpty(parentId))
            {
                return childName;
            }

            return String.Format("{0}{1}{2}", parentId, PathChar, childName);
        }

        public const string PathChar = "_";
    }

    /// <summary>
    /// A class that stores the model design for a Variable.
    /// </summary>
    public partial class VariableDesign
    {
        [XmlIgnore()]
        public object DecodedValue;

        [XmlIgnore()]
        public DataTypeDesign DataTypeNode;
    }

    /// <summary>
    /// A class that stores the model design for a VariableType.
    /// </summary>
    public partial class VariableTypeDesign
    {
        [XmlIgnore()]
        public object DecodedValue;

        [XmlIgnore()]
        public DataTypeDesign DataTypeNode;
    }

    /// <summary>
    /// A class that stores the model design for a Method.
    /// </summary>
    public partial class MethodDesign
    {
        [XmlIgnore()]
        public bool HasArguments;

        [XmlIgnore()]
        public MethodDesign MethodType;

        [XmlIgnore()]
        public MethodDesign MethodDeclarationNode;
    }

    /// <summary>
    /// A class that stores the model design for a Type.
    /// </summary>
    public partial class TypeDesign
    {
        [XmlIgnore()]
        public TypeDesign BaseTypeNode;

        public TypeDesign Copy()
        {
            return (TypeDesign)MemberwiseClone();
        }
    }

    /// <summary>
    /// A class that stores the model design for a Type.
    /// </summary>
    public partial class InstanceDesign
    {
        [XmlIgnore()]
        public TypeDesign TypeDefinitionNode;

        [XmlIgnore()]
        public InstanceDesign InstanceDeclarationNode;

        [XmlIgnore()]
        public InstanceDesign OveriddenNode;

        public InstanceDesign Copy()
        {
            return (InstanceDesign)MemberwiseClone();
        }

        [XmlIgnore()]
        public bool IdentifierRequired;
    }

    /// <summary>
    /// A class that stores a reference between nodes.
    /// </summary>
    public partial class Reference
    {
        [XmlIgnore()]
        public NodeDesign SourceNode;

        [XmlIgnore()]
        public RelativePath SourceRelativePath;

        [XmlIgnore()]
        public NodeDesign TargetNode;

        [XmlIgnore()]
        public RelativePath TargetRelativePath;
    }

    /// <summary>
    /// A class that stores a parameter for a node.
    /// </summary>
    public partial class Parameter
    {
        [XmlIgnore()]
        public NodeDesign Parent;

        [XmlIgnore()]
        public DataTypeDesign DataTypeNode;

        [XmlIgnore()]
        public bool IdentifierInName;

        [XmlIgnore()]
        public bool IsInherited;
    }

    /// <summary>
    /// The set of basic data types
    /// </summary>
    public enum BasicDataType
    {
        Boolean,
        SByte,
        Byte,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,
        Float,
        Double,
        String,
        DateTime,
        Guid,
        ByteString,
        XmlElement,
        NodeId,
        ExpandedNodeId,
        StatusCode,
        DiagnosticInfo,
        QualifiedName,
        LocalizedText,
        DataValue,
        Number,
        Integer,
        UInteger,
        Enumeration,
        Structure,
        BaseDataType,
        UserDefined
    }

    /// <summary>
    /// A class that stores the model design for a DataType.
    /// </summary>
    public partial class DataTypeDesign
    {
        [XmlIgnore()]
        public bool HasEncodings;

        [XmlIgnore()]
        public bool HasFields;

        [XmlIgnore()]
        public bool IsStructure;

        [XmlIgnore()]
        public bool IsEnumeration;

        [XmlIgnore()]
        public BasicDataType BasicDataType;
    }
}
