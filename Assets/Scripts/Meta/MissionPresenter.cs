using EntropySyndicate.Utils;
using EntropySyndicate.Data;
using EntropySyndicate.Services;
using TMPro;
using UnityEngine;

namespace EntropySyndicate.Meta
{
    public class MissionPresenter : MonoBehaviour
    {
        [SerializeField] private MetaProgressionConfig config;
        [SerializeField] private TextMeshProUGUI missionLabel;

        private SaveService _save;

        public void Initialize(SaveService save)
        {
            _save = save;
            Refresh();
        }

        public void Refresh()
        {
            if (_save == null)
            {
                return;
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder(2048);
            for (int i = 0; i < config.missions.Count; i++)
            {
                MetaProgressionConfig.MissionData mission = config.missions[i];
                int progress = _save.Data.missionProgress.GetInt(mission.id);
                builder.Append('[').Append(mission.cadence).Append("] ")
                    .Append(mission.description).Append(" ")
                    .Append(progress).Append('/').Append(mission.target).AppendLine();
            }

            missionLabel.text = builder.ToString();
        }
    }
}
