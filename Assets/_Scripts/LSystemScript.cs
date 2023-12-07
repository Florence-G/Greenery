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
            // {'F', "FF+[+F-F]-[-F+F]"},
            // {'L', "FF+[+F-L]-[-F-L]"}
            {'F', "F++F+[+F--F]-[-F+F]"},
            {'L', "F++F+[+F--L]-[-F-L]"}
        };

        StartCoroutine(GenerateTree());
    }

    private IEnumerator GenerateTree()
    {
        currentString = axiom;
        StringBuilder sb = new StringBuilder();

        Quaternion initialRotation = Quaternion.Euler(90f, 0f, 0f);

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
                        treeSegment.transform.localScale = new Vector3(1, length, 1);

                        // Vector3 leafPosition = transform.position + -10 * Vector3.up + leafFactorX * Vector3.right;
                        // Instantiate(Leaf, leafPosition, transform.rotation);
                        // Vector3 initialPosition = transform.position;
                        // transform.Translate(Vector3.up * length);

                        // GameObject treeSegment = Instantiate(Branch);
                        // treeSegment.transform.position = (initialPosition + transform.position) / 2f;
                        // treeSegment.transform.up = (transform.position - initialPosition).normalized;
                        // treeSegment.transform.localScale = new Vector3(1, length, 1);
                        // // Vector3 initialPosition = transform.position;
                        // // Vector3 forwardDirection = transform.up; // Get the forward direction in 3D space
                        // // transform.Translate(forwardDirection * length);


                        // // // Vector3 initialPosition = transform.position;
                        // // // transform.Translate(Vector3.up * length);
                        // // // Vector3 finalPosition = transform.position;

                        // // GameObject treeSegment = Instantiate(Branch);
                        // // treeSegment.transform.position = (initialPosition + finalPosition) / 2f;
                        // // treeSegment.transform.up = (finalPosition - initialPosition).normalized;
                        // // treeSegment.transform.localScale = new Vector3(1, length, 1);
                        // // GameObject treeSegment = Instantiate(Branch);
                        // // treeSegment.transform.position = initialPosition;
                        // // treeSegment.transform.LookAt(finalPosition);
                        // // float segmentLength = Vector3.Distance(initialPosition, finalPosition);
                        // // treeSegment.transform.localScale = new Vector3(1, segmentLength, 1);

                        // // Vector3 leafPosition = transform.position + -10 * Vector3.up + leafFactorX * Vector3.right;
                        // // Instantiate(Leaf, leafPosition, transform.rotation);
                        Instantiate(Leaf, transform.position, transform.rotation);
                        break;
                    case 'L':
                        Vector3 currentLeafPosition = transform.position + leafFactorY * Vector3.up + leafFactorX * Vector3.right;
                        Instantiate(Leaf, currentLeafPosition, transform.rotation);
                        break;
                    case 'X':
                        break;
                    case '+':
                        transform.Rotate(Vector3.back * angle);
                        break;
                    case '-':
                        transform.Rotate(Vector3.forward * angle);
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