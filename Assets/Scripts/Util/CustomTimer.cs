namespace Util {
    public class CustomTimer {
        private float _timeRemaining;
        private readonly float[] _stagesTimeRemaining;

        private int _currentStage = 0;
        
        private bool _isStopped;

        public CustomTimer(float seconds) {
            _timeRemaining = seconds;
            _stagesTimeRemaining = new[] { seconds };
        }

        public CustomTimer(float[] stages) {
            _stagesTimeRemaining = stages;
            _timeRemaining = _stagesTimeRemaining[0];
            _currentStage = 0;
        }

        public float GetTimeRemaining() {
            return _timeRemaining;
        }

        public void SetTimeRemaining(float seconds) {
            _timeRemaining = seconds;
        }

        public void Stop() {
            _isStopped = true;
        }

        public void Continue() {
            _isStopped = false;
        }

        public bool IsStopped() {
            return _isStopped;
        }

        public void DeltaTick(float deltaTime, out bool stageChanged, out int currentStage) {
            _timeRemaining -= deltaTime;
            
            stageChanged = false;
            currentStage = _currentStage;
            
            if (_timeRemaining > 0) return;
            currentStage = _currentStage;
            stageChanged = true;
            if (currentStage < _stagesTimeRemaining.Length) {
                _currentStage++;
                _timeRemaining = _stagesTimeRemaining[_currentStage];
            }
            _isStopped = true;
        }
    }
}