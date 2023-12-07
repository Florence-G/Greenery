using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TransformInfo
{
    public Vector3 position;
    public Quaternion rotation;
}



public class LSystemScript : MonoBehaviour
{
    [SerializeField] private int iterations = 3;
    [SerializeField] private GameObject Branch;
    [SerializeField] private GameObject Leaf;
    [SerializeField] private GameObject Pot;
    [SerializeField] private float length;
    [SerializeField] private float angle;
    private const string axiom = "X+L";
    private Stack<TransformInfo> transformStack;
    private Dictionary<char, string> rules;
    private string currentString = string.Empty;

    private void GenerateLeaves(Vector3 position)
    {
        float numberOfLeaves = Random.Range(1f, 5f);
        for (int i = 0; i < numberOfLeaves ; i++) 
        {
            float angle1 = Random.Range(0f, 360f); 
            float angle2 = Random.Range(0f, 360f);
            float angle3 = Random.Range(0f, 360f); 

            Instantiate(Leaf, position, Quaternion.Euler(angle1, angle2, angle3));
        }
    }

    void Start()
    {
        Instantiate(Pot, transform.position, transform.rotation);
        transformStack = new Stack<TransformInfo>();
        rules = new Dictionary<char, string> {
            {'X', "F+[[$X]-X]-F&[-F$X]+X&"},
            {'F', "FF"},
            {'+', "+"},
            {'-', "-"},
            {'&', "&"},
            {'$', "$"},
            {'[', "["},
            {']', "]"},
        };

        StartCoroutine(GenerateTree());
    }

    private IEnumerator GenerateTree()
    {
        currentString = axiom;
        StringBuilder sb = new StringBuilder();

        Quaternion initialRotation = Quaternion.Euler(0f, 0f, 0f);
        Vector3 initPosition = transform.position;


        for (int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }

            currentString = sb.ToString();
            sb = new StringBuilder();

            yield return new WaitForSeconds(2.5f);

            transform.rotation = initialRotation;
            transform.position = initPosition;

            foreach (char c in currentString)
            {
                switch (c)
                {
                    case 'F':
                        Vector3 initialPosition = transform.position;
                        transform.Translate(transform.up * length);

                        Vector3 finalPosition = transform.position;

                        GameObject treeSegment = Instantiate(Branch);
                        treeSegment.transform.position = (initialPosition + finalPosition) / 2f;
                        treeSegment.transform.up = transform.up;
                        treeSegment.transform.LookAt(finalPosition, transform.up);

                        treeSegment.transform.rotation = Quaternion.FromToRotation(Vector3.up, finalPosition - initialPosition);

                        treeSegment.transform.localScale = new Vector3(0.01f, length, 0.01f);

                        if(i>2){
                            GenerateLeaves(finalPosition);
                        }
                        break;

                    case '+':
                        transform.Rotate(Vector3.back * angle);
                        break;

                    case '-':
                        transform.Rotate(Vector3.forward * angle);
                        break;
                    case '&': 
                        transform.Rotate(Vector3.left * angle);
                        break;
                    case '$': 
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
                        break;
                }
            }
        }
        yield return null;
    }
}
