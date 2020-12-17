import { Component, OnInit, ViewChild, Input } from '@angular/core';

import { User } from '../../models/user.model';
import { UserEditor } from '../../models/user-editor.model';
import { Role } from '../../models/role.model';
import { UserService } from 'src/app/services/user.service';
import { UtiltityService } from 'src/app/services/utility.service';


@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent implements OnInit {

  public isEditMode = false;
  public isNewUser = false;
  public isSaving = false;
  public isChangePassword = false;
  public showValidationErrors = false;
  public uniqueId: string = UtiltityService.uniqueId();
  public user: User = new User();
  public userEditor: UserEditor = new UserEditor();
  public allRoles: Role[] = [];

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  @Input()
  isViewOnly: boolean;

  @Input()
  isGeneralEditor = false;

  @ViewChild('f')
  public form;

  @ViewChild('userName')
  public userName;

  @ViewChild('userPassword')
  public userPassword;

  @ViewChild('email')
  public email;

  @ViewChild('currentPassword')
  public currentPassword;

  @ViewChild('newPassword')
  public newPassword;

  @ViewChild('confirmPassword')
  public confirmPassword;

  @ViewChild('roles')
  public roles;

  constructor(private userService: UserService) {
  }

  ngOnInit() {
      this.loadCurrentUserData();
  }

  private loadCurrentUserData() {
    this.userService.getUsers()
      .subscribe(
        results => {
          this.onCurrentUserDataLoadSuccessful(results[0], results[1])
        },
        error => this.onCurrentUserDataLoadFailed(error)
      );
  }

  private onCurrentUserDataLoadSuccessful(user: User, roles: Role[]) {
    this.user = user;
    this.allRoles = roles;
  }

  private onCurrentUserDataLoadFailed(error: any) {
    this.user = new User();
  }

  getRoleByName(name: string) {
    return this.allRoles.find((r) => r.name === name);
  }  

  edit() {
    if (!this.isNewUser) {
      this.userEditor = new UserEditor();
      Object.assign(this.userEditor, this.user);
    } else {
      if (!this.userEditor) {
        this.userEditor = new UserEditor();
      }
    }

    this.isEditMode = true;
    this.showValidationErrors = true;
    this.isChangePassword = false;
  }

  

  cancel() {
      this.userEditor = new UserEditor();

    this.showValidationErrors = false;
    this.resetForm();

    if (this.changesCancelledCallback) {
      this.changesCancelledCallback();
    }
  }



  changePassword() {
    this.isChangePassword = true;
  }

  resetForm(replace = false) {
    this.isChangePassword = false;

    if (!replace) {
      this.form.reset();
    } else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }

  newUser(allRoles: Role[]): UserEditor {
    this.isNewUser = true;

    this.allRoles = [...allRoles];
    this.user = this.userEditor = new UserEditor();
    this.edit();

    return this.userEditor;
  }

  editUser(user: User, allRoles: Role[]): UserEditor {
    if (user) {
      this.isNewUser = false;

      this.setRoles(user, allRoles);
      this.user = new User();
      this.userEditor = new UserEditor();
      Object.assign(this.user, user);
      Object.assign(this.userEditor, user);
      this.edit();

      return this.userEditor;
    } else {
      return this.newUser(allRoles);
    }
  }

  displayUser(user: User, allRoles?: Role[]): void {

    this.user = new User();
    Object.assign(this.user, user);
    this.deletePasswordFromUser(this.user);
    this.setRoles(user, allRoles);

    this.isEditMode = false;
  }

  save() {
    this.isSaving = true;

    if (this.isNewUser) {
      this.userService.newUser(this.userEditor).subscribe(
        user => { 
          this.saveSuccessHelper(user); 
        }, 
        error => this.saveFailedHelper(error));
    } else {
      this.userService.updateUser(this.userEditor)
      .subscribe(
        response => {
          this.saveSuccessHelper()
        }, 
        error => this.saveFailedHelper(error));
    }
  }

  private saveSuccessHelper(user?: User) {
    if (user) {
      Object.assign(this.userEditor, user);
    }

    this.isSaving = false;
    this.isChangePassword = false;
    this.showValidationErrors = false;

    this.deletePasswordFromUser(this.userEditor);
    Object.assign(this.user, this.userEditor);
    this.userEditor = new UserEditor();
    this.resetForm();

    this.isEditMode = false;

    if (this.changesSavedCallback) {
      this.changesSavedCallback();
    }
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;

    if (this.changesFailedCallback) {
      this.changesFailedCallback();
    }
  }

  private setRoles(user: User, allRoles?: Role[]) {
    this.allRoles = allRoles ? [...allRoles] : [];

    if (user.roleIds) {
      for (const ur of user.roleIds) {
        if (!this.allRoles.some(r => r.id === ur)) {
          this.allRoles.unshift(new Role(ur));
        }
      }
    }
  }
  
  close() {
    this.userEditor = this.user = new UserEditor();
    this.showValidationErrors = false;
    this.resetForm();
    this.isEditMode = false;

    if (this.changesSavedCallback) {
      this.changesSavedCallback();
    }
  }

  deletePasswordFromUser(user: UserEditor | User) {
    const userEditor = user as UserEditor;

    delete userEditor.currentPassword;
    delete userEditor.newPassword;
    delete userEditor.confirmPassword;
  }
}
