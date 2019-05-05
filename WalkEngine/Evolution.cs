using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WalkEngine
{
    public sealed class Evolution : INotifyPropertyChanged
    {
        private readonly IGenerationProducer _generationProducer;
        
        private int _modelsToTakeFromGeneration = 1;
        private double _targetDistance = 100;
        private bool _finished = false;
        private int _generationNumber = 0;
        private IReadOnlyList<WalkModelDescription> _bestModels = Array.Empty<WalkModelDescription>();
        
        public Evolution(IGenerationProducer generationProducer)
        {
            _generationProducer = generationProducer ?? throw new ArgumentNullException(nameof(generationProducer));
        }

        public int ModelsToTakeFromGeneration
        {
            get => _modelsToTakeFromGeneration;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Number of models to take from generation must be positive");
                if (_modelsToTakeFromGeneration == value)
                    return;
                _modelsToTakeFromGeneration = value;
                OnPropertyChanged();
            }
        }

        public double TargetDistance
        {
            get => _targetDistance;
            set
            {
                if (value <= 0 || double.IsNaN(value) || double.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(value), "Target distance must be positive finite number");
                if (_targetDistance == value)
                    return;
                _targetDistance = value;
                OnPropertyChanged();
            }
        }

        public bool Finished
        {
            get => _finished;
            private set
            {
                if (_finished == value)
                    return;
                _finished = value;
                OnPropertyChanged();
            }
        }

        public int GenerationNumber
        {
            get => _generationNumber;
            private set
            {
                _generationNumber = value;
                OnPropertyChanged();
            }
        }

        public IReadOnlyList<WalkModelDescription> BestModels
        {
            get => _bestModels;
            private set
            {
                _bestModels = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ProduceNextGeneration()
        {
            BestModels = GetBestModels();
            GenerationNumber++;
            
            var bestModel = BestModels.FirstOrDefault();
            Finished = bestModel == null || (bestModel.IsAlive && bestModel.Distance >= TargetDistance);
        }

        private List<WalkModelDescription> GetBestModels()
        {
            var candidates = GenerationNumber == 0?
                _generationProducer.Init(ModelsToTakeFromGeneration) :
                _generationProducer.GetNextGeneration(BestModels);

            return candidates
                .Select(GetFinalProgress)
                .OrderByDescending(finalProgress => Math.Floor(finalProgress.Distance))
                .ThenBy(finalProgress => finalProgress.IsAlive ? 0 : 1)
                .ThenBy(finalProgress => finalProgress.Time)
                .ThenByDescending(finalProgress => finalProgress.Distance)
                .Take(ModelsToTakeFromGeneration)
                .Select(finalProgress => new WalkModelDescription(finalProgress))
                .ToList();
        }

        private WalkProgress GetFinalProgress(WalkModel walkModel)
        {
            const double TimeFrame = 10.0;
            const double MinAcceptableSpeed = 5.0;

            var progress = new WalkProgress(walkModel);

            while(true)
            {
                var timeFrameEnd = progress.Time + TimeFrame;
                var maxDistance = progress.Distance;

                while (progress.Time < timeFrameEnd && progress.MoveForward())
                {
                    if (progress.Distance > TargetDistance)
                        return progress;

                    maxDistance = Math.Max(maxDistance, progress.Distance);
                }
                
                if (!progress.IsAlive || maxDistance < progress.Time * MinAcceptableSpeed)
                    return progress;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}