namespace Visitapp.Application.DTOs.Leader;

public class LeaderFamilyDto
{
    public int FamilyId { get; set; }
    public string FamilyName { get; set; }
    public int VisitsCompleted { get; set; }
    public int VisitsPending { get; set; }
    public DateTime? LastVisitDate { get; set; }
    public List<string> Notes { get; set; }
}
