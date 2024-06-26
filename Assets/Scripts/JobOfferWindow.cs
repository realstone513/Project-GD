namespace Realstone
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class JobOfferWindow : GenericWindow
    {
        public TextMeshProUGUI recruitCostText;
        public List<GameObject> employeeInfoList = new();
        private List<GameObject> employeeList = new();
        private List<GameObject> logs = new();
        private EmployeeGrade grade = EmployeeGrade.Beginner;
        private WorkType workType = WorkType.Planner;
        private int cost = 10;
        private int offerIndex = -1;
        public GameObject offerWindow;
        public GameObject negotiateWindow;
        public TextMeshProUGUI firstLog;
        public GameObject logPrefab;
        public Transform logTransform;
        public TextMeshProUGUI salaryText;
        public Slider salarySlider;
        public TextMeshProUGUI bonusText;
        public Slider bonusSlider;
        public Button submitButton;
        public Button confirmButton;
        private int submitCount;

        private void Start()
        {
            bonusSlider.minValue = 0;
            bonusSlider.maxValue = 10000;
        }

        private void OnEnable()
        {
            negotiateWindow.SetActive(false);
            offerWindow.SetActive(true);
            InfoActive(false);
        }

        public void ChangeCostText(Int32 num)
        {
            GameManager gm = GameManager.instance;
            switch (num)
            {
                case 0:
                    cost = gm.gameRule.jobOfferCostBeginner;
                    grade = EmployeeGrade.Beginner;
                    bonusSlider.maxValue = gm.money < 10000 ? gm.money : 10000;
                    break;
                case 1:
                    cost = gm.gameRule.jobOfferCostIntermediate;
                    grade = EmployeeGrade.Intermediate;
                    bonusSlider.maxValue = gm.money < 20000 ? gm.money : 20000;
                    break;
                case 2:
                    cost = gm.gameRule.jobOfferCostExpert;
                    grade = EmployeeGrade.Expert;
                    bonusSlider.maxValue = gm.money < 50000 ? gm.money : 50000;
                    break;
            }
            recruitCostText.text = $"{cost}";
        }

        public void ChangeWorkType(Int32 num)
        {
            switch (num)
            {
                case 0:
                    workType = WorkType.Planner;
                    break;
                case 1:
                    workType = WorkType.Developer;
                    break;
                case 2:
                    workType = WorkType.Artist;
                    break;
            }
        }

        public void Offer()
        {
            if (GameManager.instance.money <= cost)
                return;

            offerIndex = -1;
            CreateNewEmployees();
            InfoActive(true);
            negotiateWindow.SetActive(false);
            offerWindow.SetActive(true);
            submitCount = 0;
            submitButton.interactable = true;
            confirmButton.interactable = false;
            bonusSlider.value = 0;
            salarySlider.value = 0;
            ChangeSalaryValue();
            ChangeBonusValue();
        }

        public void SelectInfo(int idx)
        {
            offerIndex = idx;
            offerWindow.SetActive(false);
            negotiateWindow.SetActive(true);
            Employee thisEmployee = employeeList[offerIndex].GetComponent<Employee>();
            GameRule rule = GameManager.instance.gameRule;
            (int min, int max) = Utils.GetIntRange(rule.averageSalary[(int)thisEmployee.grade] * 1.1f, rule.salaryRangeRatio);
            salarySlider.minValue = min;
            salarySlider.maxValue = max;
            salarySlider.value = thisEmployee.fakeSalary;
            bonusSlider.value = 0;
            FirstLog(thisEmployee);
            foreach (var log in logs)
                Destroy(log);
            InfoActive(false);
        }

        private void FirstLog(Employee thisEmployee)
        {
            firstLog.text = $"{thisEmployee.empName}은(는) {thisEmployee.fakeSalary}만원을 원합니다.";
        }

        public void ChangeSalaryValue()
        {
            salaryText.text = $"연봉: {salarySlider.value:0.}만원";

            confirmButton.interactable = false;
        }

        public void ChangeBonusValue()
        {
            bonusText.text = $"사이닝 보너스: {bonusSlider.value:0.}만원";

            confirmButton.interactable = false;
        }

        public void Submit()
        {
            submitCount++;
            if (submitCount == 5)
                submitButton.interactable = false;

            GameObject newLog = Instantiate(logPrefab, logTransform);
            logs.Add(newLog);

            Employee thisEmployee = employeeList[offerIndex].GetComponent<Employee>();

            int proposalBalance = (int)salarySlider.value + (int)bonusSlider.value / 2;
            // Debug.Log(proposalBalance);
            TextMeshProUGUI logText = newLog.GetComponent<TextMeshProUGUI>();
            if (proposalBalance > thisEmployee.fakeSalary)
            {
                CanConfirm(true);
                logText.color = Color.green;
                logText.text = $"{submitCount}/5 {thisEmployee.empName}은(는) 만족합니다.";
                if (thisEmployee.fakeSalary <= thisEmployee.salary)
                    thisEmployee.fakeSalary = thisEmployee.salary;
            }
            else if (proposalBalance > thisEmployee.salary * 0.95f)
            {
                CanConfirm(false);
                logText.color = Color.yellow;
                logText.text = $"{submitCount}/5 {thisEmployee.empName}은(는) 아쉬워합니다.";
                thisEmployee.fakeSalary -= (int)(Math.Abs(thisEmployee.fakeSalary - proposalBalance) * 0.5f);
                if (thisEmployee.fakeSalary <= thisEmployee.salary)
                    thisEmployee.fakeSalary = thisEmployee.salary;
            }
            else
            {
                CanConfirm(false);
                logText.color = Color.red;
                logText.text = $"{submitCount}/5 {thisEmployee.empName}은(는) 불쾌해합니다.";
            }
            FirstLog(thisEmployee);
        }

        public void Confirm()
        {
            GameManager gm = GameManager.instance;
            gm.employeeManager.AddToUnassign(employeeList[offerIndex]);
            Employee employee = employeeList[offerIndex].GetComponent<Employee>();
            // Debug.Log($"fake: {employee.fakeSalary} real: {employee.salary}");
            employee.salary = (int)salarySlider.value;
            gm.TranslateGameMoney(-(int)bonusSlider.value);
            gm.financeLossDictionary.Add($"{employee.empName} 월급",
                gm.CalculateMonthSalary(employee.salary));
            foreach (var log in logs)
                Destroy(log);
            Close();
            ClearList();
        }

        private void CanConfirm(bool value)
        {
            confirmButton.interactable = value;
        }

        private void CreateNewEmployees()
        {
            ClearList();

            GameManager.instance.TranslateGameMoney(-cost);
            for (int i = 0; i < 3; i++)
            {
                employeeList.Add(GameManager.instance.employeeManager.CreateNewEmployee(grade, workType));
                employeeInfoList[i].GetComponent<EmployeeInfo>().SetInfo(employeeList[i].GetComponent<Employee>(), true);
            }
        }

        private void ClearList()
        {
            int size = employeeList.Count;
            for (int i = 0; i < size; i++)
            {
                if (offerIndex != i)
                    Destroy(employeeList[i]);
            }

            employeeList.Clear();
        }

        private void InfoActive(bool value)
        {
            foreach (var info in employeeInfoList)
                info.SetActive(value);
            if (offerIndex != -1)
                employeeInfoList[offerIndex].SetActive(true);
        }

        private void OnDisable()
        {
            ClearList();
            offerIndex = -1;
        }
    }
}