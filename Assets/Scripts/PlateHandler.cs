using UnityEngine;

public class PlateHandler : AnimAttacher
{
    [SerializeField][Range(0, 1)] private float _fireCauseProbabiliy;
    [Header("Sockets")]
    [SerializeField] private Transform _plateHandSocket;
    [SerializeField] private Transform _foodSocket;

    [Header("Props")]
    [SerializeField] private GameObject _plate;
    [SerializeField] private GameObject[] _fireCauseFoods;
    [SerializeField] private GameObject[] _fireNonCauseFoods;


    public override void HideAll()
    {
        _plate.SetActive(false);
        foreach (GameObject foodObj in _fireCauseFoods)
        {
            foodObj.SetActive(false);
        }
        foreach (GameObject foodObj in _fireNonCauseFoods)
        {
            foodObj.SetActive(false);
        }
    }


    private GameObject ChooseFood()
    {
        float randValue = UnityEngine.Random.Range(0f, 1f);
        return (randValue < _fireCauseProbabiliy)
                ? _fireCauseFoods.GetRandom()
                : _fireNonCauseFoods.GetRandom();
    }


    public void LiftPlate()
    {
        HideAll();
        AttachProp(_plate, _plateHandSocket);
        _plate.SetActive(true);
        GameObject choosedFood = ChooseFood();
        AttachProp(choosedFood, _foodSocket);
        choosedFood.SetActive(true);
    }
}
