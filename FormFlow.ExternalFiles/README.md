## Pose-Detection Analysis Output

After a video is uploaded to the `/analyze` endpoint, the system automatically analyzes the athlete’s movement using a pose-detection model.  
The analysis is based on detecting body keypoints in each video frame and computing joint angles from these keypoints.

The result is returned as a structured JSON object.

#### Libraries

*MediaPipe*
*Detect Body Keypoints*

*OpenCV*
*Read and decode the video and convert the BGR into RGB format so MediaPipe can process it.*

*NumPy*
*Performs numerical calculations like computing joint angles between keypoints.*

### Detected Body Keypoints

```json
"detectedKeypoints": 15
```

The system uses a pose-detection model that identifies key anatomical points of the human body (such as shoulders, elbows, hips, knees, and ankles).  
For the analysis, a fixed set of **15 relevant keypoints** is selected.

These keypoints form the basis for all further calculations and ensure that the movement can be analyzed consistently across different videos.

### Calculated Joint Angles

```json
"angles": {
  "knee": {
    "minDeg": 72.4,
    "maxDeg": 168.9
  },
  "elbow": {
    "maxDeg": 143.2
  }
}
```

The system calculates joint angles by forming vectors between three body keypoints:
- **Knee angle**  
    Calculated from the hip, knee, and ankle points.  
    This angle represents how much the knee is bent or extended during the movement.
- **Elbow angle** 
    Calculated from the shoulder, elbow, and wrist points.  
    This angle represents the degree of arm extension.
    
The angles are measured in **degrees**:
- `minDeg` indicates the smallest angle observed (maximum bending)
- `maxDeg` indicates the largest angle observed (maximum extension)
### Detected Technical Issues

```json
"technicalErrors": [
  {
    "code": "KNEE_TOO_STRAIGHT",
    "message": "Leg remains almost fully extended during movement."
  }
]
```

Based on the calculated joint angles, the system checks for common technical issues using predefined threshold values.

Examples of detected issues include:
- The knee remains almost fully extended during the movement
- The knee is not bent enough to generate sufficient force
- The arm is not fully extended during execution

Each issue is derived directly from the measured angles and described using a short message that explains the problem in plain language.
