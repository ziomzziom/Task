public class Loan

{

  public int LoanId { get; set; }

  public Account Account { get; set; }

  public BankEmployee BankEmployee { get; set; }

  public decimal LoanAmount { get; set; }

  public decimal InterestRate { get; set; }

  public DateTime LoanStartDate { get; set; }

  public DateTime LoanEndDate { get; set; }

}



public class Account

{

  public int AccountId { get; set; }

  public Customer Customer { get; set; }

  public string AccountType { get; set; }

  public decimal AccountBalance { get; set; }

  public DateTime AccountOpenDate { get; set; }

}



public class Customer

{

  public int CustomerId { get; set; }

  public string CustomerName { get; set; }

  public string CustomerAddress { get; set; }

  public string CustomerPhoneNumber { get; set; }

  public DateTime DateOfBirth { get; set; }

  public bool IsActive { get; set; }

}



public class BankEmployee

{

  public int EmployeeId { get; set; }

  public string EmployeeName { get; set; }

  public BankDepartment BankDepartment { get; set; }

  public DateTime HireDate { get; set; }

}



public class BankDepartment

{

  public int DepartmentId { get; set; }

  public string DepartmentName { get; set; }

  public string PlacementCity { get; set; }

}