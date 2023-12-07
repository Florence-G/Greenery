
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

public class TransformInfo
{
    public Vector3 position;
    public Quaternion rotation;
}

public class LSystemScript : MonoBehaviour
{
    [SerializeField] private int iterations = 4;
    [SerializeField] private GameObject Branch;
    [SerializeField] private GameObject Leaf;
    [SerializeField] private float length = 0.05f;
    [SerializeField] private float angle = 20f;
    [SerializeField] private int leafFactorY = -5;
    [SerializeField] private int leafFactorX = -10;
    private const string axiom = "F";
    private Stack<TransformInfo> transformStack;
    private Dictionary<char, string> rules;
    private string currentString = string.Empty;

    void Start()
    {
        transformStack = new Stack<TransformInfo>();
        rules = new Dictionary<char, string> {
            // {'F', "F+[+F]-[-F+D]"},
            // {'L', "F+[+F-DL]-[-F-L]"},
            // {'F', "F+[+F]-[-F]"},
            // {'L', "F+[+F-DL]-[-F-L]"},
            {'F', "F+[+F]-[-F]"},
{'L', "F+[+F-DL]-[-F-LL]"},
        };

        StartCoroutine(GenerateTree());
    }

    private IEnumerator GenerateTree()
    {
        currentString = axiom;
        StringBuilder sb = new StringBuilder();

        Quaternion initialRotation = Quaternion.Euler(0f, 0f, 0f);

        for (int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }

            currentString = sb.ToString();
            sb = new StringBuilder();

            yield return new WaitForSeconds(0.5f);

            transform.rotation = initialRotation;

            foreach (char c in currentString)
            {
                switch (c)
                {
                    case 'F':

                        Vector3 initialPosition = transform.position;
                        transform.Translate(Vector3.up * length);
                        Vector3 finalPosition = transform.position;

                        GameObject treeSegment = Instantiate(Branch);
                        treeSegment.transform.position = (initialPosition + finalPosition) / 2f;
                        treeSegment.transform.up = (finalPosition - initialPosition).normalized;
                        treeSegment.transform.localScale = new Vector3(0.01f, length, 0.01f);

                        Instantiate(Leaf, finalPosition, transform.rotation);
                        break;
                    case 'L':
                        Vector3 currentLeafPosition = transform.position + leafFactorY * Vector3.up + -1 * Vector3.right + -5 * transform.forward;
                        Instantiate(Leaf, transform.position, transform.rotation);
                        break;
                    case 'X':
                        break;
                    case '+':
                        transform.Rotate(Vector3.back * angle);
                        break;
                    case '-':
                        transform.Rotate(Vector3.forward * angle);
                        break;
                    case 'D':
                        transform.Rotate(Vector3.left * angle);
                        break;
                    case 'G':
                        transform.Rotate(Vector3.right * angle);
                        break;
                    case '[':
                        transformStack.Push(new TransformInfo()
                        {
                            position = transform.position,
                            rotation = transform.rotation,
                        });
                        break;
                    case ']':
                        TransformInfo ti = transformStack.Pop();
                        transform.position = ti.position;
                        transform.rotation = ti.rotation;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid L-Tree operation");
                }
            }
        }
        yield return null;
    }

}