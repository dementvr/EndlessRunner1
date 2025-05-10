using System.Collections;                // Дозволяє використовувати інтерфейс IEnumerator
using System.Collections.Generic;        // Дозволяє використовувати Queue<T> та інші generic-колекції
using UnityEngine;                       // Підключає Unity API (MonoBehaviour, GameObject, Transform тощо)

public class SegmentGenerator : MonoBehaviour   // Оголошуємо клас-скрипт, успадкований від MonoBehaviour
{
    [Header("Префаби сегментів")]
    public GameObject[] segment;           // 1) Масив префабів сегментів, які ми будемо інстанціювати

    [Header("Налаштування генерації")]
    [SerializeField] int zPos = 50;       // 2) Початкова Z-позиція першого сегмента (бачимо й міняємо в Інспекторі)
    public float segmentLength = 50f;     // 3) Скільки одиниць уздовж Z займає один сегмент
    public float spawnInterval = 15f;     // 4) Інтервал між спавнами (у секундах)

    [Header("Трансформ гравця")]
    public Transform playerTransform;     // 5) Посилання на Transform персонажа (щоб знати його Z-координату)

    // 6) Створюємо чергу, щоб зберігати всі активні сегменти в порядку їх появи
    private Queue<GameObject> activeSegments = new Queue<GameObject>();
    // 7) Прапорець, який не дає запускати корутину SegmentGen повторно, поки вона ще не завершилася
    private bool creatingSegment = false;

    void Update()
    {
        // 8) Якщо ми зараз не створюємо сегмент, запускаємо корутину (тобто починаємо новий спавн)
        if (!creatingSegment)
        {
            creatingSegment = true;        // 8.1) Виставляємо прапорець, щоб інші кадри не стартували ту ж корутину
            StartCoroutine(SegmentGen());  // 8.2) Запускаємо корутину для створення одного сегмента
        }

        // 9) Якщо в черзі є хоча б один сегмент, перевіряємо, чи варто його видалити
        if (activeSegments.Count > 0)
        {
            GameObject first = activeSegments.Peek();  // 9.1) Отримуємо посилання на найстаріший сегмент (без видалення)
            // 9.2) Порівнюємо Z-позицію гравця і сегмента:
            // якщо гравець пройшов далі за початок сегмента + його довжину → сегмент позаду
            if (playerTransform.position.z - first.transform.position.z > segmentLength)
            {
                Destroy(first);            // 9.3) Видаляємо цей сегмент із гри
                activeSegments.Dequeue();  // 9.4) Прибираємо його з черги, щоб не зберігати зайві об’єкти
            }
        }
    }

    // 10) Корутіна, яка створює один сегмент, чекає, а потім знімає прапорець
    IEnumerator SegmentGen()
    {
        // 10.1) Вибираємо випадковий індекс у межах 0…(кількість префабів–1)
        int segmentNum = Random.Range(0, segment.Length);

        // 10.2) Обчислюємо точку спавна (X=0, Y=0, Z=zPos)
        Vector3 spawnPos = new Vector3(0f, 0f, zPos);
        // 10.3) Створюємо копію префаба в сцені
        GameObject seg = Instantiate(segment[segmentNum], spawnPos, Quaternion.identity);

        // 10.4) Додаємо новий сегмент в кінець черги
        activeSegments.Enqueue(seg);

        // 10.5) Зсуваємо zPos вперед на довжину сегмента, щоб наступний з’явився далі
        zPos += (int)segmentLength;

        // 10.6) Чекаємо заданий інтервал перед тим, як дозволити наступний спавн
        yield return new WaitForSeconds(spawnInterval);
        // 10.7) Скидаємо прапорець, щоб корутина знову могла запуститися в Update()
        creatingSegment = false;
    }
}
