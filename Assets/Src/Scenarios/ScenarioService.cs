using System.Collections.Generic;

namespace Suburb.Scenarios
{
    public class ScenarioService
    {
        private readonly HashSet<BaseScenario> scenarios = new();

        public bool Login(BaseScenario scenario)
        {
            if (!scenarios.Contains(scenario))
            {
                scenarios.Add(scenario);
                return true;
            }

            return false;
        }

        public bool Logout(BaseScenario scenario)
        {
            return scenarios.Remove(scenario);
        }

        // мы должны дать метод, позволяющий другим назначить на сценарий интерактивный обьект
    }
}
