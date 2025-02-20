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
            Transform t = transform.GetChild(i);
            PieceInfo info = new PieceInfo(t.position, t.rotation, t, t.gameObject.GetComponent<Rigidbody>());
            pieces.Add(info);
        }
    }

    // Método chamado quando há colisão com um trigger
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Player colidiu com a plataforma");
            StartCoroutine(BreakPlataform()); // Inicia a coroutine de destruição da plataforma
        }
    }

    // Coroutine que gerencia a destruição e respawn das peças
    private IEnumerator BreakPlataform() {
        yield return new WaitForSeconds(waitTime); // Espera pelo tempo especificado antes de destruir

        // Destrói a plataforma
        foreach (PieceInfo piece in pieces) {
            piece.rigidbody.isKinematic = false; // Desativa o modo kinematic do rigidbody

            // Gera forças aleatórias para aplicar nas peças
            float x = UnityEngine.Random.Range(minForce, maxForce);
            float y = UnityEngine.Random.Range(minForce, maxForce);
            float z = UnityEngine.Random.Range(minForce, maxForce);

            piece.rigidbody.AddForce(x, y, z, ForceMode.Force); // Aplica a força ao rigidbody
        }

        yield return new WaitForSeconds(respawnTime); // Espera pelo tempo especificado antes do respawn

        // Respawna a plataforma
        foreach (PieceInfo piece in pieces) {
            piece.rigidbody.isKinematic = true; // Reativa o modo kinematic do rigidbody
            piece.transform.position = piece.startPosition; // Restaura a posição inicial
            piece.transform.rotation = piece.startRotation; // Restaura a rotação inicial
        }
    }
}