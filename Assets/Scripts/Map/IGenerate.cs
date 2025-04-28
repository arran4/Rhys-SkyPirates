using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Interface class to allow iteration on diffrent highlight needs.
public interface IGenerate
{
    public Board Generate(Map Data);

}
