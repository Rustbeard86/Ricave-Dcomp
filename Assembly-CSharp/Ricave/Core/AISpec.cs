using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ricave.Core
{
    public class AISpec : Spec
    {
        public AINode Root
        {
            get
            {
                return this.root;
            }
        }

        public bool HasShouldFleeNode
        {
            get
            {
                if (this.cachedHasShouldFleeNode == null)
                {
                    this.cachedHasShouldFleeNode = new bool?(AISpec.< get_HasShouldFleeNode > g__Check | 5_0(this.root));
                }
                return this.cachedHasShouldFleeNode.Value;
            }
        }

        [CompilerGenerated]
        internal static bool <get_HasShouldFleeNode>g__Check|5_0(AINode node)
		{
			if (node is AINode_ShouldFlee)
			{
				return true;
			}
			AINode_FirstAvailable ainode_FirstAvailable = node as AINode_FirstAvailable;
			if (ainode_FirstAvailable != null)
			{
				using (List<AINode>.Enumerator enumerator = ainode_FirstAvailable.Nodes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (AISpec.<get_HasShouldFleeNode>g__Check|5_0(enumerator.Current))
						{
							return true;
						}
					}
}
return false;
			}
			return false;
		}

		[Saved]
private AINode root;

private bool? cachedHasShouldFleeNode;
	}
}