using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert {
	public Alert(string _name, string _sentence) {
        name = _name;
        sentence = _sentence;
    }
	public string name;
	[TextArea(3, 10)]
	public string sentence;
}
