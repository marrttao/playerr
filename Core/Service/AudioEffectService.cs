
using NAudio.Wave;
using playerr.Domain;
using System.Collections.Generic;

namespace playerr.Core.Service
{
    public class AudioEffectService
    {
        private readonly List<IAudioEffect> effects = new();

        public void AddEffect(IAudioEffect effect)
        {
            effects.Add(effect);
        }

        public void RemoveEffect(IAudioEffect effect)
        {
            effects.Remove(effect);
        }

        public ISampleProvider ApplyEffects(ISampleProvider input)
        {
            ISampleProvider result = input;

            foreach (var effect in effects)
                result = effect.Apply(result);

            return result;
        }
    }
}