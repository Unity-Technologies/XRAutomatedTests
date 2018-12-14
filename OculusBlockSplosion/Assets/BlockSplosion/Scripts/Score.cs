using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
	public static int StarsLeft		{ get; set; }
	public static int MaxStars		{ get; set; }

	public static int ShotsFired	{ get; set; }
	public static int ThreeStars	{ get; set; }
	public static int TwoStars		{ get; set; }

	public static bool Completed	{ get { return !InfinitePlay && StarsLeft <= 0; } }

	public static bool InfinitePlay	{ get; set; }
	public static bool TitleScreen  { get; set; }

	[SerializeField]
	private TextMesh	_text;

	private void Update()
	{
		string text;

		if ( InfinitePlay || TitleScreen )
		{
			text = "";
		}
		else
		{
			text = string.Format("Collected: {0, 2}/{1, 2}\nShots: {2, 2}", MaxStars - StarsLeft, MaxStars, ShotsFired);
			if (Completed)
			{
				int stars = ShotsFired <= ThreeStars ? 3 : ShotsFired <= TwoStars ? 2 : 1;
				string starsText = "";
				starsText = starsText.PadRight(stars, '*');
				starsText = starsText.PadRight(3, '-');
				text += "\nStars: " + starsText;
			}
		}
		_text.text = text;
	}
}
