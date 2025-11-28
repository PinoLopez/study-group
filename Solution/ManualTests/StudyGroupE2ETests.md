Manual E2E Test Cases for Study Group Feature

1 Create Study Group – Valid (Positive, Happy Path)

Level: E2E Manual
Steps:  

    Log in as a student user.  
    Navigate to “Create Study Group” page.  
    Enter valid name: “Math Masters” (20 chars).  
    Select subject: Math.  
    Click “Create”.
    Inputs: Name = "Math Masters", Subject = Math
    Expectations:

    Success message shown.  
    New group appears in “All Study Groups” list.  
    Creation date is visible and recent.
    Add to regression: Yes (core user flow)

2 Create Study Group – Name Too Short (Negative)

Level: E2E Manual
Steps:  

    Log in.  
    Go to “Create Study Group”.  
    Enter name: "Math" (4 chars).  
    Select subject: Physics.  
    Click “Create”.
    Inputs: Name = "Math", Subject = Physics
    Expectations:

    Error message: “Name must be between 5 and 30 characters.”  
    Group not created.
    Add to regression: Yes (enforces AC1.a)

3 Create Study Group – Name Too Long (Negative)

Level: E2E Manual
Steps:  

    Log in.  
    Go to “Create Study Group”.  
    Enter name: "A".Repeat(31) (e.g., “AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA”).  
    Select subject: Chemistry.  
    Click “Create”.
    Inputs: Name = 31-character string, Subject = Chemistry
    Expectations:

    Error message about name length.  
    Group not created.
    Add to regression: Yes (AC1.a)

4 Create Study Group – Invalid Subject via UI (Negative)

Level: E2E Manual
Steps:  

    Log in.  
    Attempt to create group.  
    (If subject is a dropdown) → only Math/Chem/Physics available → skip.  
    (If free text) → type "Biology".  
    Submit.
    Inputs: Subject = "Biology" (if allowed by UI)
    Expectations:

    Error: “Subject must be Math, Chemistry, or Physics.”  
    Group not created.
    Add to regression: Yes (AC1.b)  

5 Prevent Duplicate Study Group for Same Subject (Negative)

Level: E2E Manual
Steps:  

    Create a Study Group for Math (valid).  
    Attempt to create another group for Math.  
    Submit.
    Inputs: Name = "Math Team", Subject = Math (second time)
    Expectations:

    Error: “A study group for Math already exists.”  
    Second group not created.
    Add to regression: Yes (AC1 uniqueness)

6 Join Study Group (Positive)

Level: E2E Manual
Steps:  

    Log in as User A.  
    View list of Study Groups.  
    Click “Join” on a Physics group.  
    Confirm.
    Inputs: Valid study group ID (visible in UI)
    Expectations:

    Success message: “You’ve joined the group.”  
    User A appears in group member list.
    Add to regression: Yes (AC2)

7 Join Non-Existent Group (Negative)

Level: E2E Manual
Steps:  

    Manually navigate to /join?groupId=99999 (or use dev tools to simulate).  
    Attempt to join.
    Inputs: Invalid groupId
    Expectations:

    Error: “Study group not found.”  
    No change in user’s groups.
    Add to regression: Yes (defensive UX)

8 View All Study Groups (Positive)

Level: E2E Manual
Steps:  

    Log in.  
    Go to “Study Groups” page.  
    Observe list.
    Inputs: None
    Expectations:

    All existing groups displayed (name, subject, creation date, member count).
    Add to regression: Yes (AC3)

9 Filter Study Groups by Subject (Positive)

Level: E2E Manual
Steps:  

    Go to Study Groups page.  
    Select filter: Subject = “Chemistry”.  
    Apply filter.
    Inputs: Filter = Chemistry
    Expectations:

    Only Chemistry groups shown.  
    Math/Physics groups hidden.
    Add to regression: Yes (AC3.a)

10 Sort Study Groups by Creation Date (Positive)

Level: E2E Manual
Steps:  

    Ensure ≥2 groups exist with different creation dates.  
    On Study Groups page, select sort: “Newest First”.  
    Observe order.  
    Switch to “Oldest First”.
    Inputs: Sort option: Newest / Oldest
    Expectations:

    Groups re-ordered correctly by CreateDate.  
    Newest at top (or bottom) as selected.
    Add to regression: Yes (AC3.b)

11 Leave Study Group (Positive)

Level: E2E Manual
Steps:  

    Join a group (if not already).  
    Go to group detail or your profile.  
    Click “Leave Group”.  
    Confirm.
    Inputs: Valid group you’ve joined
    Expectations:

    Success message.  
    Group no longer in your list.  
    Member count decreases by 1.
    Add to regression: Yes (AC4)

12 Leave Group You Never Joined (Negative)

Level: E2E Manual
Steps:  

    Log in as User B.  
    Navigate to a group User B never joined.  
    Attempt to leave (via URL manipulation or hidden button).
    Inputs: Valid groupId, but user not a member
    Expectations:

    Either:  
        Button not visible (best), OR  
        Error: “You are not a member of this group.”
        Add to regression: No (edge case, low risk)

13 Cross-Browser / Responsive Check 

Level: E2E Manual
Steps:  

    Test creation/joining on Chrome, Firefox, mobile.
    Expectations:

    Consistent behavior and layout.
    Add to regression: Yes (if multi-device support required)

Summary

    Total manual test cases: 13  
    Regression-critical: 11 (all core ACs covered)  
    Focus: Validation, uniqueness, join/leave, filtering, sorting  
    Assumption: UI exists with forms, lists, filters, and sort controls