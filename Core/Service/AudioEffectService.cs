using NAudio.Wave;
using System.Collections.Generic;

public interface IAudioEq
{
    ISampleProvider Apply(ISampleProvider input);
}

namespace playerr.Core.Service
{
    public class AudioEffectService
    {
        private readonly List<IAudioEq> effects = new();

        public void AddEffect(IAudioEq effect)
        {
            effects.Add(effect);
        }

        public void RemoveEffect(IAudioEq effect)
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