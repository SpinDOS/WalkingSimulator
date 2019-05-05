using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using WalkEngine;

namespace WalkVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private bool _evolutionWasFinished = false;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        
        public MainWindow()
        {
            _timer.Tick += TriggerEvolutionStep;
            PropertyChanged += OnEvolutionChanged;
            InitializeComponent();
        }

        public Evolution Evolution { get; private set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void GenerationProducerCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var modelsToTakeFromGeneration = Evolution?.ModelsToTakeFromGeneration ?? 4;
            var generationProducerType = (Type) args.AddedItems.Cast<ComboBoxItem>().Single().Tag;
            Evolution = new Evolution((IGenerationProducer)Activator.CreateInstance(generationProducerType))
            {
                TargetDistance = WalkModelVisualizer.GetTargetDistance(this),
                ModelsToTakeFromGeneration = modelsToTakeFromGeneration,
            };
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Evolution)));
        }

        private void OnEvolutionChanged(object sender, PropertyChangedEventArgs args)
        {
            _evolutionWasFinished = false;
            ChangeTimerInterval(true);
        }

        private void TriggerEvolutionStep(object sender, EventArgs args)
        {
            WalkModelVisualizer.SetGenerationTime(this, double.NaN); // disable updates of model visualizers
            
            Evolution.ProduceNextGeneration();

            if (Evolution.BestModels.Count == 0)
            {
                _timer.Stop();
                return;
            }
            
            var maxGenerationTime = Evolution.BestModels.Max(modelDescription => modelDescription.Time);
            if (Evolution.Finished && !_evolutionWasFinished)
            {
                _evolutionWasFinished = true;
                WalkModelVisualizer.SetAnimationDuration(this, TimeSpan.FromSeconds(maxGenerationTime));
                ChangeTimerInterval(false);
            }
            
            WalkModelVisualizer.SetGenerationTime(this, maxGenerationTime); // enable updates of model visualizers
        }

        private void OnAnimationDurationChanged(object sender, DataTransferEventArgs e) => ChangeTimerInterval(true);

        private void ChangeTimerInterval(bool triggerEvolutionImmediate)
        {
            _timer.Stop();
            if (Evolution == null)
                return;
            
            _timer.Interval = WalkModelVisualizer.GetAnimationDuration(this);
            if (triggerEvolutionImmediate)
                TriggerEvolutionStep(null, null);
            _timer.Start();
        }

        internal class GenerationProducersSource
        {
            public List<ComboBoxItem> GetGenerationProducers()
            {
                return ReflectionHelper.GetImplementationTypes(typeof(IGenerationProducer))
                    .OrderBy(OrderAttribute.GetOrder)
                    .ThenBy(type => type.Name)
                    .ThenBy(type => type.FullName)
                    .Select(type => new ComboBoxItem() {Content = type.Name, Tag = type})
                    .ToList();
            }
        }
    }
}