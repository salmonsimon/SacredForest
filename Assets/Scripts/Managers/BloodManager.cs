using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BloodManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> bloodPrefabs = new List<GameObject>();

    public void Bleed(Transform parent, Vector3 position)
    {
        List<GameObject> toBleed = new List<GameObject>();
        List<GameObject> prefabsToChoose = new List<GameObject>(bloodPrefabs);

        int firstRandomParticle = Random.Range(0, prefabsToChoose.Count);
        toBleed.Add(prefabsToChoose[firstRandomParticle]);
        prefabsToChoose.Remove(prefabsToChoose[firstRandomParticle]);

        int secondRandomParticle = Random.Range(0, prefabsToChoose.Count);
        toBleed.Add(prefabsToChoose[secondRandomParticle]);
        prefabsToChoose.Remove(prefabsToChoose[secondRandomParticle]);

        int thirdRandomParticle = Random.Range(0, prefabsToChoose.Count);
        toBleed.Add(prefabsToChoose[thirdRandomParticle]);
        prefabsToChoose.Remove(prefabsToChoose[thirdRandomParticle]);

        StartCoroutine(BleedAndDestroy(toBleed, parent, position));
    }

    private IEnumerator BleedAndDestroy(List<GameObject> toBleed, Transform parent, Vector3 position)
    {
        List<GameObject> bloodParticles = new List<GameObject>();

        foreach (GameObject bloodParticle in toBleed)
        {
            GameObject newBloodParticle = Instantiate(bloodParticle, position, Quaternion.identity);
            newBloodParticle.transform.SetParent(parent);
            bloodParticles.Add(newBloodParticle);
        }

        yield return new WaitForSeconds(.5f);

        foreach (GameObject bloodParticle in bloodParticles)
        {
            Destroy(bloodParticle.gameObject);
        }
    }
}
