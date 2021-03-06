﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using db_mapping;

public partial class Web_Forms_User_actions_Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
        {
            PasswordTextBox.Attributes.Add("value", PasswordTextBox.Text);
        }

    }
    
    protected void emailServerValidate(object source, ServerValidateEventArgs args)
    {
        Regex email_regular_expression = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        if (email_regular_expression.IsMatch(args.Value) && args.Value.Length > 0 && !DatabaseFunctions.checkEmailIfExist(args.Value))
        {
            args.IsValid = true;
            emailformgroup.Attributes["class"] = "form-group";
            emailformgroup.Controls.Remove(emailformgroup.FindControl("glyphicon"));
        }
        else
        {
            args.IsValid = false;
            emailformgroup.Attributes["class"] = "form-group has-error has-feedback";
            EmailTextBox.Attributes["aria-describedby"] = "inputError2Status";
            HtmlGenericControl span = new HtmlGenericControl("span");
            span.Attributes["class"] = "glyphicon glyphicon-remove form-control-feedback";
            span.Attributes["aria-hidden"] = "true";
            span.Attributes["id"] = "glyphicon";
            emailformgroup.Controls.Add(span);
        }

    }
    
    protected void passwordServerValidate(object source, ServerValidateEventArgs args)
    {
        if (args.Value.Length > 3 && args.Value.Length < 20)
        {
            args.IsValid = true;
            passwordformgroup.Attributes["class"] = "form-group";
            passwordformgroup.Controls.Remove(emailformgroup.FindControl("glyphicon"));
        }
        else
        {
            args.IsValid = false;
            passwordformgroup.Attributes["class"] = "form-group has-error has-feedback";
            PasswordTextBox.Attributes["aria-describedby"] = "inputError2Status";
            HtmlGenericControl span = new HtmlGenericControl("span");
            span.Attributes["class"] = "glyphicon glyphicon-remove form-control-feedback";
            span.Attributes["aria-hidden"] = "true";
            span.Attributes["id"] = "glyphicon";
            passwordformgroup.Controls.Add(span);
        }
    }
   
    protected void confirmPasswordServerValidate(object source, ServerValidateEventArgs args)
    {
        if (args.Value.Length > 3 && args.Value.Length < 21 && args.Value == PasswordTextBox.Text)
        {
            args.IsValid = true;
            confirmpasswordformgroup.Attributes["class"] = "form-group";
            confirmpasswordformgroup.Controls.Remove(emailformgroup.FindControl("glyphicon"));
        }
        else
        {
            args.IsValid = false;
            confirmpasswordformgroup.Attributes["class"] = "form-group has-error has-feedback";
            PasswordTextBox.Attributes["aria-describedby"] = "inputError2Status";
            HtmlGenericControl span = new HtmlGenericControl("span");
            span.Attributes["class"] = "glyphicon glyphicon-remove form-control-feedback";
            span.Attributes["aria-hidden"] = "true";
            span.Attributes["id"] = "glyphicon";
            confirmpasswordformgroup.Controls.Add(span);
        }
    }

    protected void registerButtonClick(object sender, EventArgs e)
    {
        Page.Validate();
        if (Page.IsValid)
        {
            User user = new User();
            string email = EmailTextBox.Text;
            string password = sha256(PasswordTextBox.Text);
            string first_name = (String.IsNullOrEmpty(FirstNameTextBox.Text)) ? null : FirstNameTextBox.Text;
            string last_name = (String.IsNullOrEmpty(LastNameTextBox.Text)) ? null : LastNameTextBox.Text;
            List<string> specifics_list = new List<string>();
            foreach (ListItem specific in SpecificCheckBoxList.Items)
                if (specific.Selected == true)
                    specifics_list.Add(specific.Text);
            if (specifics_list.Count == 0)
                specifics_list = null;
            user.Initialize(
                -100000, 
                email, 
                password, 
                first_name, 
                last_name, 
                DateTime.Now,
                "CLIENT",
                specifics_list);
            int id = DatabaseFunctions.insertUser(user);
            user.Initialize(
                id, 
                email, 
                password, 
                first_name, 
                last_name, 
                DateTime.Now,
                "CLIENT",
                specifics_list);
            Session["user"] = user;
            Response.Redirect("../../Web_Forms/Master/Waiter.aspx");
        }
    }

    private string sha256(string password)
    {
        SHA256Managed crypt = new SHA256Managed();
        string hash = String.Empty;
        byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
        foreach (byte bit in crypto)
        {
            hash += bit.ToString("x2");
        }
        return hash;
    }
}