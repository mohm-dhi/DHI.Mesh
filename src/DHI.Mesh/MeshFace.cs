﻿
using System;
using System.Diagnostics;

namespace DHI.Mesh
{
  /// <summary>
  /// A face is a boundary between two elements. The face has a direction, defined from <see cref="FromNode"/> to <see cref="ToNode"/>,
  /// and in that direction it has a <see cref="LeftElement"/> and a <see cref="RightElement"/>
  /// <para>
  /// For boundary faces, the <see cref="RightElement"/> does not exist and is null.
  /// </para>
  /// </summary>
  [DebuggerDisplay("MeshFace: {FromNode.Index}-{ToNode.Index} ({LeftElement.Index},{RightElement?.Index})")]
  public class MeshFace
  {
    /// <summary> From node - start point of face </summary>
    public MeshNode FromNode { get; set; }
    /// <summary> to node - end point of face </summary>
    public MeshNode ToNode { get; set; }
    /// <summary> Left element </summary>
    public MeshElement LeftElement { get; set; }
    /// <summary> Right element. For boundary faces this is null. </summary>
    public MeshElement RightElement { get; set; }

    /// <summary>
    /// Boundary code.
    /// <para>
    /// For internal faces this is zero. For boundary faces this is the boundary code on the face.
    /// </para>
    /// </summary>
    public int Code { get; set; }


    /// <summary>
    /// Evaluate its boundary code based on the code of the nodes.
    /// </summary>
    internal void SetBoundaryCode()
    {
      // If the RightElement exists, this face is an internal face
      if (RightElement != null)
      {
        return;
      }

      // RightElement does not exist, so it is a boundary face.
      int fromCode = FromNode.Code;
      int toCode   = ToNode.Code;

      // True if "invalid" boundary face, then set it as internal face.
      bool internalFace = false;

      if (fromCode == 0)
      {
        internalFace = true;
        throw new Exception(string.Format(
          "Invalid mesh: Boundary face, from node {0} to node {1} is missing a boundary code on node {0}. " +
          "Hint: Modify boundary code for node {0}",
          FromNode.Index + 1, ToNode.Index + 1));
      }

      if (toCode == 0)
      {
        internalFace = true;
        throw new Exception(string.Format(
          "Invalid mesh: Boundary face, from node {0} to node {1} is missing a boundary code on node {1}. " +
          "Hint: Modify boundary code for node {1}",
          FromNode.Index + 1, ToNode.Index + 1));
      }

      int faceCode;

      // Find face code:
      // 1) In case any of the nodes is a land node (code value 1) then the
      //    boundary face is a land face, given boundary code value 1.
      // 2) For boundary faces (if both fromNode and toNode have code values larger than 1), 
      //    the face code is the boundary code value of toNode.
      if (fromCode == 1 || toCode == 1)
        faceCode = 1;
      else
        faceCode = toCode;

      if (!internalFace)
        Code = faceCode;
    }

  }
}
