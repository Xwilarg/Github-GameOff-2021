using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Bug.UI
{
	public class ItemRow : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _titleLabel;
		[SerializeField] private TextMeshProUGUI _subtitleLabel;
		[SerializeField] private Image _icon;

		public TextMeshProUGUI TitleLabel => _titleLabel;
		public TextMeshProUGUI SubtitleLabel => _subtitleLabel;
		public Image Icon => _icon;
	}
}
