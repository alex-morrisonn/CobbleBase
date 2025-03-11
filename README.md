# Cobble

Cobble is a secure platform designed to help organizations or groups manage member information in one convenient, centralized location. It features a robust base system that can be expanded or modified to meet each organization’s unique requirements, all while maintaining strong security and granular data access control.

---

## Core Purpose

- **Centralized Information Storage**  
  Cobble enables organizations to securely store and manage member details in one place.

- **Easy Customization**  
  Although Cobble has a standard set of features, it can be easily adapted to various sectors by adding specialized modules.

- **Controlled Data Access**  
  Every database interaction runs through an API layer, ensuring each organization only has access to the data it needs.

---

## Example Use Cases

1. **Sporting Organizations**  
   - Collect extra medical information (e.g., doctor’s details, blood type, allergies) so authorized personnel can quickly access critical data in case of injuries.  
   - Restrict access to ensure only approved individuals can view sensitive health information.

2. **Universities**  
   - Categorize users (students, tutors, lecturers) and further segment them by classes or project groups.  
   - Tutors can assign tasks or distribute assignments; students can track due dates and group members.  
   - Use API-based data retrieval so each tutor or lecturer can only access relevant student information.

---

## Hierarchical Control

A key aspect of Cobble is its built-in hierarchy and permission structure:

- **Administrators**  
  Manage who can be added, who can create new groups, and which types of information are visible to different roles.

- **User Permissions**  
  Ensure that each organization’s members see only the data they are authorized to view.

---

## User Interface & Account Management

Cobble provides a straightforward, intuitive UI:

1. **Account Creation & Login**  
   Users sign up or log in with an email and password.

2. **Required Details**  
   During registration, users must provide at least one email address, a phone number, one or more physical addresses (if applicable), and a password.

3. **User Dashboard**  
   Once logged in, users can view and update their personal information.

4. **Organization-Specific Pages**  
   Each organization’s custom requirements (e.g., medical details for sports clubs or assignment schedules for universities) appear in a dedicated section.

---

## Technical Specifications

1. **Language & Documentation**  
   - Built with C#.  
   - Clear code structure and detailed documentation allow new developers to easily continue development.

2. **API-Centric Architecture**  
   - All database interactions occur via secure API calls.  
   - Organizations only receive data relevant to their users and needs, preventing unauthorized access.

3. **Version Control & Collaboration**  
   - Code is stored in a GitHub repository, streamlining collaboration and handovers.  
   - No sensitive data is hardcoded; environment variables and config values are securely managed.

4. **Database & Deployment**  
   - Member information is stored in a secure database.  
   - The database and application run online via Docker and Kubernetes, hosted on a platform like DigitalOcean.  
   - Ensures high availability, easy scalability, and a clear separation of responsibilities for each organization’s data access.

---

## Summary

Cobble provides a universal solution for organizations seeking a secure, adaptable platform to manage member data. By enforcing strict API-based data access, maintaining hierarchical controls, and offering an intuitive user interface, Cobble can be seamlessly adapted to meet a wide range of requirements while upholding strong security and privacy standards.