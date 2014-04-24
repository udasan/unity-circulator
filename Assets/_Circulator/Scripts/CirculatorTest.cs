using UnityEngine;
using System.Collections;

public class CirculatorTest : MonoBehaviour
{
	Circulator circulator_;

	void Start ()
	{
		circulator_ = Component.FindObjectOfType<Circulator>();
	}

	void OnGUI ()
	{
		if (!circulator_) { return; }

		for (int i = 0; i < circulator_.maxCirculators; i++) {
			for (int j = (int)Circulator.Command.Stop; j <= (int)Circulator.Command.Strong; j++) {
				Rect rect = new Rect((200 * i + 20), (60 * (j - (int)Circulator.Command.Stop) + 20), 180, 40);
				Circulator.Command command = (Circulator.Command)j;
				if (GUI.Button(rect, string.Format("Circulator {0} : {1}", i, command))) {
					circulator_.Circulate(i, command);
				}
			}
		}
	}
}
