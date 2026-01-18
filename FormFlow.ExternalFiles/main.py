from fastapi import FastAPI, UploadFile, File
import cv2, tempfile, os
import mediapipe as mp
import numpy as np

app = FastAPI()

mp_pose = mp.solutions.pose
pose = mp_pose.Pose(static_image_mode=False)

# -----------------------------
# Utility: compute joint angle
# -----------------------------
def angle(a, b, c):
    a, b, c = map(np.array, (a, b, c))
    ba = a - b
    bc = c - b
    cos_angle = np.dot(ba, bc) / (np.linalg.norm(ba) * np.linalg.norm(bc))
    cos_angle = np.clip(cos_angle, -1.0, 1.0)
    return float(np.degrees(np.arccos(cos_angle)))

@app.post("/analyze")
async def analyze(file: UploadFile = File(...)):
    # Save uploaded video temporarily
    with tempfile.NamedTemporaryFile(delete=False, suffix=".mp4") as tmp:
        tmp.write(await file.read())
        video_path = tmp.name

    cap = cv2.VideoCapture(video_path)

    # Required keypoints (>= 15)
    REQUIRED_LANDMARKS = [
        0,   # nose
        11,  # left shoulder
        12,  # right shoulder
        13,  # left elbow
        14,  # right elbow
        15,  # left wrist
        16,  # right wrist
        23,  # left hip
        24,  # right hip
        25,  # left knee
        26,  # right knee
        27,  # left ankle
        28,  # right ankle
        31,  # left foot
        32   # right foot
    ]

    knee_angles = []
    elbow_angles = []

    while cap.isOpened():
        success, frame = cap.read()
        if not success:
            break

        rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        result = pose.process(rgb)

        if not result.pose_landmarks:
            continue

        lm = result.pose_landmarks.landmark

        # Left knee angle
        knee = angle(
            (lm[23].x, lm[23].y),  # hip
            (lm[25].x, lm[25].y),  # knee
            (lm[27].x, lm[27].y)   # ankle
        )
        knee_angles.append(knee)

        # Left elbow angle
        elbow = angle(
            (lm[11].x, lm[11].y),  # shoulder
            (lm[13].x, lm[13].y),  # elbow
            (lm[15].x, lm[15].y)   # wrist
        )
        elbow_angles.append(elbow)

    cap.release()
    os.remove(video_path)

    # Aggregate metrics
    max_knee = max(knee_angles) if knee_angles else None
    min_knee = min(knee_angles) if knee_angles else None
    max_elbow = max(elbow_angles) if elbow_angles else None

    # -----------------------------
    # Technical error detection
    # -----------------------------
    errors = []

    if max_knee and max_knee > 160:
        errors.append({
            "code": "KNEE_TOO_STRAIGHT",
            "message": "Leg remains almost fully extended during movement."
        })

    if min_knee and min_knee < 70:
        errors.append({
            "code": "INSUFFICIENT_KNEE_BEND",
            "message": "Knee flexion is too small for effective force generation."
        })

    if max_elbow and max_elbow < 150:
        errors.append({
            "code": "ARM_NOT_EXTENDED",
            "message": "Arm extension is insufficient during execution."
        })

    return {
        "detectedKeypoints": len(REQUIRED_LANDMARKS),
        "angles": {
            "knee": {
                "minDeg": min_knee,
                "maxDeg": max_knee
            },
            "elbow": {
                "maxDeg": max_elbow
            }
        },
        "technicalErrors": errors
    }