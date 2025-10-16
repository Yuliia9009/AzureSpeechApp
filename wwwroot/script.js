
let recorder;
let audioContext;
let gumStream;
let isRecording = false;

function toggleRecording() {
    const recordBtn = document.getElementById("recordBtn");
    const indicator = document.getElementById("recordingIndicator");

    if (!isRecording) {
        navigator.mediaDevices.getUserMedia({ audio: true }).then(stream => {
            audioContext = new (window.AudioContext || window.webkitAudioContext)();
            gumStream = stream;
            const input = audioContext.createMediaStreamSource(stream);
            recorder = new Recorder(input, { numChannels: 1 });
            recorder.record();
            isRecording = true;

            recordBtn.textContent = "⏹ Остановить";
            indicator.style.display = "inline";
        }).catch(err => {
            alert("Ошибка доступа к микрофону: " + err);
        });
    } else {
        recorder.stop();
        gumStream.getAudioTracks()[0].stop();
        recorder.exportWAV(blob => {
            const formData = new FormData();
            formData.append("file", blob, "recording.wav");
            formData.append("language", document.getElementById("language").value);

            fetch("/api/speech/recognize", {
                method: "POST",
                body: formData
            })
            .then(res => res.json())
            .then(data => {
                document.getElementById("recognizedText").value = data.text;
            });

            document.getElementById("recordBtn").textContent = "🎙 Говорить";
            document.getElementById("recordingIndicator").style.display = "none";
            isRecording = false;
        });
    }
}

async function uploadFile() {
    const fileInput = document.getElementById("fileInput");
    const language = document.getElementById("language").value;

    if (!fileInput.files.length) {
        alert("Выберите аудиофайл!");
        return;
    }

    const formData = new FormData();
    formData.append("file", fileInput.files[0]);
    formData.append("language", language);

    console.log("🔄 Загружается файл:", fileInput.files[0]);
    console.log("🌐 Язык распознавания:", language);

    try {
        const res = await fetch("/api/speech/recognize", {
            method: "POST",
            body: formData
        });

        if (!res.ok) {
            const errorText = await res.text();
            console.error("❌ Ошибка ответа сервера:", res.status, errorText);
            alert("Ошибка " + res.status + ": " + errorText);
            return;
        }

        const data = await res.json();
        console.log("✅ Ответ сервера:", data);
        document.getElementById("recognizedText").value = data.text;
    } catch (err) {
        console.error("❗ Исключение при отправке файла:", err);
        alert("Произошла ошибка при загрузке файла.");
    }
}

async function translateText() {
    const text = document.getElementById("recognizedText").value;
    const toLang = document.getElementById("translateTo").value;

    const res = await fetch("/api/speech/translate", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ text: text, to: toLang })
    });

    const data = await res.json();
    document.getElementById("translatedText").value = data.translated;
}

function downloadText() {
    const original = document.getElementById("recognizedText").value.trim();
    const translated = document.getElementById("translatedText").value.trim();

    let content = "";

    if (original && translated) {
        content = `🗣 Оригинал:\n${original}\n\n🌍 Перевод:\n${translated}`;
    } else if (original) {
        content = original;
    } else if (translated) {
        content = translated;
    } else {
        alert("Нет текста для сохранения.");
        return;
    }

    const blob = new Blob([content], { type: "text/plain" });
    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = "result.txt";
    link.click();
}
