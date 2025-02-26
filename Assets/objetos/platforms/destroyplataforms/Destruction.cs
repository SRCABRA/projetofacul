using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Estrutura que armazena informações de uma peça
public struct PieceInfo {
    public Vector3 startPosition; // Posição inicial da peça
    public Quaternion startRotation; // Rotação inicial da peça

    public Transform transform; // Transform da peça
    public Rigidbody rigidbody; // Rigidbody da peça

    // Construtor da estrutura PieceInfo
    public PieceInfo(Vector3 startPosition, Quaternion startRotation, Transform transform, Rigidbody rigidbody) {
        this.startPosition = startPosition;
        this.startRotation = startRotation;
        this.transform = transform;
        this.rigidbody = rigidbody;
    }
}

// Classe que gerencia a destruição e respawn das peças
public class Destruction : MonoBehaviour {
    public float waitTime; // Tempo de espera antes da destruição
    public float respawnTime; // Tempo de espera antes do respawn

    public float minForce; // Força mínima aplicada às peças
    public float maxForce; // Força máxima aplicada às peças

    private List<PieceInfo> pieces = new List<PieceInfo>(); // Lista de informações das peças

    // Método chamado no início
    private void Start() {
        // Itera sobre todos os filhos do transform e adiciona suas informações na lista
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null) {
                pieces.Add(new PieceInfo(child.position, child.rotation, child, rb));
            }
        }
    }

    // Método para destruir as peças
    public void DestroyPieces() {
        StartCoroutine(DestroyPiecesCoroutine());
    }

    private IEnumerator DestroyPiecesCoroutine() {
        yield return new WaitForSeconds(waitTime);

        foreach (PieceInfo piece in pieces) {
            piece.rigidbody.isKinematic = false;
            piece.rigidbody.AddForce(Vector3.up * UnityEngine.Random.Range(minForce, maxForce), ForceMode.Impulse);
        }

        yield return new WaitForSeconds(respawnTime);

        RespawnPieces();
    }

    // Método para respawn das peças
    private void RespawnPieces() {
        foreach (PieceInfo piece in pieces) {
            piece.transform.position = piece.startPosition;
            piece.transform.rotation = piece.startRotation;
            piece.rigidbody.isKinematic = true;
        }

        Debug.Log("Peças respawnadas");
    }

    // Método chamado quando há colisão com um trigger
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Player colidiu com a plataforma");
            StartCoroutine(ChangeColorAndBreakPlatform()); // Inicia a coroutine de mudança de cor e destruição da plataforma
        }
    }

    // Coroutine que gerencia a mudança de cor e destruição das peças
    private IEnumerator ChangeColorAndBreakPlatform() {
        float elapsedTime = 0f;
        float duration = waitTime; // Duração da mudança de cor

        // Itera sobre todas as peças e armazena suas cores originais
        Dictionary<PieceInfo, Color> originalColors = new Dictionary<PieceInfo, Color>();
        foreach (PieceInfo piece in pieces) {
            Renderer renderer = piece.transform.GetComponent<Renderer>();
            if (renderer != null) {
                originalColors[piece] = renderer.material.color;
            }
        }

        // Gradualmente muda a cor das peças para vermelho
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            foreach (PieceInfo piece in pieces) {
                Renderer renderer = piece.transform.GetComponent<Renderer>();
                if (renderer != null) {
                    renderer.material.color = Color.Lerp(originalColors[piece], Color.red, t);
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(waitTime - duration); // Espera pelo tempo restante antes de destruir

        // Destrói a plataforma
        foreach (PieceInfo piece in pieces) {
            piece.rigidbody.isKinematic = false; // Desativa o modo kinematic do rigidbody

            // Gera forças aleatórias para aplicar nas peças
            Vector3 force = new Vector3(
                UnityEngine.Random.Range(minForce, maxForce),
                UnityEngine.Random.Range(minForce, maxForce),
                UnityEngine.Random.Range(minForce, maxForce)
            );
            piece.rigidbody.AddForce(force);
        }
    }
}