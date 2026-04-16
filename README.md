# TheDigitalCookbook

## 1) Home page
### Purpose
Introduce the app and guide users to log in or register.

### What it should show
- app name/logo
- short description
- buttons for:
    - **Login**
    - **Register**
    - maybe **Browse recipes** if public access is allowed

---

## 2) Register page
### Purpose
Let a new user create an account.

### Fields
- username
- password
- confirm password

### What it should do
- validate input
- send registration data to backend
- show success or error messages

---

## 3) Login page
### Purpose
Let existing users sign in.

### Fields
- username
- password

### What it should do
- validate input
- send login request to backend
- store auth state if login succeeds
- redirect to dashboard or recipe list

---

## 4) Recipe dashboard / recipe list page
### Purpose
Show all recipes for the logged-in user.

### What it should show
- list of recipe cards
- recipe name
- category
- prep time / cook time
- buttons to:
    - view
    - edit
    - delete

### What it should do
- fetch recipes from backend
- allow searching or filtering later

---

## 5) Recipe details page
### Purpose
Show one recipe in full detail.

### What it should show
- recipe name
- category
- ingredients
- instructions
- prep time
- cook time
- total time
- URL if available

### What it should do
- load one recipe by ID
- allow edit/delete if user owns it

---

## 6) Create recipe page
### Purpose
Let users add a new recipe.

### Fields
- name
- category
- ingredients
- instructions
- prep time
- cook time
- URL

### What it should do
- validate form
- send recipe data to backend
- redirect after success

---

## 7) Edit recipe page
### Purpose
Let users modify an existing recipe.

### What it should do
- load existing recipe data into form
- let user update fields
- send update request to backend

---

## 8) Profile or account page
### Purpose
Show basic user info and account actions.

### What it could include
- username
- account creation date
- logout button

---

## 9) Navigation / layout
You should also build shared UI parts:

- header
- footer
- nav menu
- login/logout button
- responsive layout for mobile

---

## Recommended build order
I’d build it in this order:

1. **Home page**
2. **Register page**
3. **Login page**
4. **Recipe list/dashboard**
5. **Create recipe page**
6. **Recipe details page**
7. **Edit recipe page**
8. **Profile page**
9. **Polish and styling**

---

## Best first version
If you want to keep it simple at first, build only these pages:

- Home
- Register
- Login
- Recipe List
- Create Recipe
- Recipe Details