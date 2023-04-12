using System;
using NuCharacter.Infrastrucure.Commands.Base;

 

namespace NuCharacter.Infrastrucure.Commands
{
    internal class LambdaCommand : Command
    {
        private readonly Action<object> _execute;
        private readonly Func<object,bool> _canExecute;
        public LambdaCommand(Action<object> Execute, Func<object,bool> CanExecute = null)
        {
            _execute = Execute ?? throw new ArgumentNullException(nameof(Execute)) ; //проверка на введенность значения
            _canExecute = CanExecute;
        }
        public override bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true; //если не передана команда, то все равно true
        

        public override void Execute(object parameter) => _execute(parameter);
       
    }
}
