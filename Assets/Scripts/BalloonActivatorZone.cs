using System.Collections;
using UnityEngine;

public class BalloonActivatorZone : MonoBehaviour
{
    [Header("Balloon Settings")]
    [Tooltip("Tag used by the balloons (e.g. 'Balloon')")]
    public string balloonTag = "Balloon";

    [Tooltip("Minimum upward force applied to balloons (used by roofs)")]
    public float minUpForce = 3f;

    [Tooltip("Maximum upward force applied to balloons (used by roofs)")]
    public float maxUpForce = 6f;

    [Tooltip("Minimum lateral force applied to balloons (used by walls)")]
    public float minSideForce = 2f;

    [Tooltip("Maximum lateral force applied to balloons (used by walls)")]
    public float maxSideForce = 5f;

    [Tooltip("Global delay between activations for ALL zones")]
    public float activationDelay = 0.5f;

    [Header("Zone Type")]
    [Tooltip("Choose whether this collider is a Roof or a Wall")]
    public ActivatorType activatorType = ActivatorType.Roof;

    [Tooltip("Direction of the force applied by this zone")]
    public Direction direction = Direction.Middle;

    // Controla o cooldown global entre todos os colliders
    private static float lastGlobalActivationTime = 0f;

    // Impede múltiplas ativações enquanto a mão ainda está dentro
    private bool handInside = false;

    // 🔒 Flag global para saber se os balões já foram liberados
    private static bool baloesLiberados = false;

    public enum ActivatorType
    {
        Roof,
        Wall
    }

    public enum Direction
    {
        Left,
        Middle,
        Right
    }

    void Start()
    {
        // 🔹 Ao iniciar, todos os balões ficam parados (sem física)
        if (!baloesLiberados)
        {
            GameObject[] balloons = GameObject.FindGameObjectsWithTag(balloonTag);
            foreach (GameObject balloon in balloons)
            {
                Rigidbody rb = balloon.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.isKinematic = true; // impede movimento
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        // Garante que só responde à Layer "Hand"
        if (other.gameObject.layer != LayerMask.NameToLayer("Hand"))
            return;

        // Ajuste instantâneo de volume ao encostar a mão
        AudioManager audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager != null)
        {
            // Aumenta instantaneamente (por exemplo, volume 1.0)
            // Espera 2s e depois reduz gradualmente para o volume original em 2s
            audioManager.AumentarVolumeInstantaneo(1f, 2f, 2f);
        }

        if (handInside) return;
        handInside = true;

        // Se ainda não liberou os balões → libera todos agora
        if (!baloesLiberados)
        {
            LiberarBaloes();
            baloesLiberados = true;
            return; // primeira ativação apenas solta os balões
        }

        // Se já foram liberados → segue comportamento normal (empurrar)
        if (Time.time - lastGlobalActivationTime < activationDelay) return;
        lastGlobalActivationTime = Time.time;

        ActivateBalloons();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hand"))
            handInside = false;
    }

    private void ActivateBalloons()
    {
        GameObject[] balloons = GameObject.FindGameObjectsWithTag(balloonTag);

        foreach (GameObject balloon in balloons)
        {
            Rigidbody rb = balloon.GetComponent<Rigidbody>();
            if (rb == null || rb.isKinematic) continue;

            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

            Vector3 force = Vector3.zero;

            if (activatorType == ActivatorType.Roof)
            {
                float upForce = Random.Range(minUpForce, maxUpForce);
                float sideForce = 0f;

                switch (direction)
                {
                    case Direction.Left:
                        sideForce = -Random.Range(1f, 2f);
                        break;
                    case Direction.Right:
                        sideForce = Random.Range(1f, 2f);
                        break;
                    case Direction.Middle:
                        sideForce = 0f;
                        break;
                }

                force = new Vector3(sideForce, upForce, 0f);
            }
            else if (activatorType == ActivatorType.Wall)
            {
                float sideForce = 0f;

                if (direction == Direction.Left)
                    sideForce = -Random.Range(minSideForce, maxSideForce);
                else if (direction == Direction.Right)
                    sideForce = Random.Range(minSideForce, maxSideForce);

                float upForce = Random.Range(0.5f, 1.5f);
                force = new Vector3(sideForce, upForce, 0f);
            }

            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    // Libera todos os balões para começarem a se mover
    private void LiberarBaloes()
    {
        GameObject[] balloons = GameObject.FindGameObjectsWithTag(balloonTag);

        foreach (GameObject balloon in balloons)
        {
            Rigidbody rb = balloon.GetComponent<Rigidbody>();
            if (rb == null) continue;

            rb.isKinematic = false; // ativa física
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }

        Debug.Log("Balões liberados!");
    }

    private IEnumerator ReduzirVolumeAposTempo(AudioManager audioManager, float volumeOriginal, float atraso)
    {
        yield return new WaitForSeconds(atraso);
        audioManager.AjustarVolumeGradualmente(volumeOriginal, 2f);
    }

}
