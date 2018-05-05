using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue  {
	public Dialogue(string _name, string[] _sentences) {
        name = _name;
        sentences = _sentences;
    }
	public string name;
	[TextArea(3, 10)]
	public string[] sentences;
}
