using System.Threading.Tasks;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioPlayer))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void SwitchScenes(int scene)
    {
        StartCoroutine(DelayLoadScene(scene));
    }

    IEnumerator DelayLoadScene(int scene)
    {
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(scene);
    }
    public void Unpause()
    {
        GameManager.GetManager().TogglePause();
    }
    public async void DelaySwitchScenes(int scene)
    {
        float timer = 0;
        float duration = .75f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            await Task.Yield();
        }

        SceneManager.LoadScene(scene);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioPlayer audioPlayer = GetComponent<AudioPlayer>();
        audioPlayer.PlaySound("BUTTON_HOVER");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioPlayer audioPlayer = GetComponent<AudioPlayer>();
        audioPlayer.PlaySound("BUTTON_CLICK");
    }
}
