using System.Collections.Generic;

namespace Infogroup.IDMS.SICCodes.Dtos
{
    public class TreeNode
    {
        public string Label { get; set; }
        public NodeData Data { get; set; }
        public string Icon { get; set; }
        public string ExpandedIcon { get; set; }
        public string CollapsedIcon { get; set; }
        public bool Leaf { get; set; }
        public bool Expanded { get; set; }
        public string StyleClass { get; set; }
        public string Key { get; set; }
        public List<TreeNode> Children { get; set; }
    }
}