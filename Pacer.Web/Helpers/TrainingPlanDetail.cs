public static class TrainingPlanDetails
{
    public static string GetTrainingDetails(string targetRace)
    {
        return targetRace switch
        {
            "BHalfMarathon" => @"
                <li>This plan starts at 7 miles in the first week and peaks at 18 miles before tapering.
                <li>It gradually introduces different types of workouts to not overwhelm a beginner.
                <li>It is designed to be completed in 10 weeks.",
            "HalfMarathon" => @"
                <li>This plan starts at 10 miles in the first week and peaks at 25 miles before tapering.
                <li>It is designed for seasoned runners who are looking to optimize their performance.
                <li>It is designed to be completed in 12 weeks.",
            "BMarathon" => @"
                <li>This plan starts at 15 miles in the first week and peaks at 38 miles before tapering.
                <li>It gradually introduces different types of workouts to not overwhelm a beginner.
                <li>It is designed to be completed in 16 weeks.",
            "Marathon" => @"<li>This plan starts at 25 miles in the first week and peaks at 50 miles before tapering.
                <li>It is an advanced marathon training plan is designed for seasoned runners who are looking to optimize their performance.
                <li>It is designed to be completed in 16 weeks.", 
            _ => "",
        };
    }
}