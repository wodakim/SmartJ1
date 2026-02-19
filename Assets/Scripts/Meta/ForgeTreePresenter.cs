using EntropySyndicate.Utils;
using EntropySyndicate.Data;
using EntropySyndicate.Services;
using TMPro;
using UnityEngine;

namespace EntropySyndicate.Meta
{
    public class ForgeTreePresenter : MonoBehaviour
    {
        [SerializeField] private MetaProgressionConfig config;
        [SerializeField] private TextMeshProUGUI listLabel;

        private ProgressionService _progression;
        private SaveService _save;

        public void Initialize(ProgressionService progression, SaveService save)
        {
            _progression = progression;
            _save = save;
            Refresh();
        }

        public void UpgradeNode(string id)
        {
            _progression.UpgradeForgeNode(id);
            Refresh();
        }

        public void Refresh()
        {
            if (listLabel == null || _save == null)
            {
                return;
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder(1024);
            for (int i = 0; i < config.forgeNodes.Count; i++)
            {
                MetaProgressionConfig.ForgeNodeData node = config.forgeNodes[i];
                int level = _save.Data.forgeLevels.GetInt(node.id);
                builder.Append(node.title).Append(" Lv.").Append(level).Append('/').Append(node.maxLevel).AppendLine();
            }

            listLabel.text = builder.ToString();
        }
    }
}
