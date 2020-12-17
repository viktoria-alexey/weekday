import { AfterViewInit, Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { forkJoin } from 'rxjs';
import { Role } from 'src/app/models/role.model';
import { UserEditor } from 'src/app/models/user-editor.model';
import { User } from 'src/app/models/user.model';
import { UserInfoComponent } from '../controls/user-info/user-info.component';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-users-management',
  templateUrl: './users-management.component.html',
  styleUrls: ['./users-management.component.css']
})
export class UsersManagementComponent implements OnInit, AfterViewInit {

  columns: any[] = [];
  rows: User[] = [];
  rowsCache: User[] = [];
  editedUser: UserEditor;
  sourceUser: UserEditor;
  editingUserName: { name: string };

  allRoles: Role[] = [];

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.columns = [
      { prop: 'index', name: '#', width: 40, cellTemplate: this.indexTemplate, canAutoResize: false },
      { prop: 'jobTitle', name: 'Title', width: 50 },
      { prop: 'userName', name: 'UserName', width: 90 },
      { prop: 'email', name: 'Email', width: 140 },
      { prop: 'roleIds', name: 'Roles', width: 120, cellTemplate: this.rolesTemplate },
      { prop: 'phoneNumber', name: 'PhoneNumber', width: 100 }
    ];

    this.columns.push({ name: '', width: 160, cellTemplate: this.actionsTemplate, resizeable: false, canAutoResize: false, sortable: false, draggable: false });

    this.loadData();
  }

  ngAfterViewInit() {
    this.userEditor.changesSavedCallback = () => {
      this.editorModal.hide();
      this.loadData();
    };

    this.userEditor.changesCancelledCallback = () => {
      this.editedUser = null;
      this.sourceUser = null;
      this.editorModal.hide();
    };
  }

  @ViewChild('indexTemplate', { static: true })
  indexTemplate: TemplateRef<any>;

  @ViewChild('userNameTemplate', { static: true })
  userNameTemplate: TemplateRef<any>;

  @ViewChild('rolesTemplate', { static: true })
  rolesTemplate: TemplateRef<any>;

  @ViewChild('editorModal', { static: true })
  editorModal: ModalDirective;

  @ViewChild('userEditor', { static: true })
  userEditor: UserInfoComponent;

  @ViewChild('actionsTemplate', { static: true })
  actionsTemplate: TemplateRef<any>;

  loadData() {

    forkJoin([
      this.userService.getUsers(),
      this.userService.getRoles()
    ])
      .subscribe(results => {
        this.onDataLoadSuccessful(results[0], results[1]);
      });
  }


  onDataLoadSuccessful(users: User[], roles: Role[]) {

    users.forEach((user, index) => {
      (user as any).index = index + 1;
    });

    this.rowsCache = [...users];
    this.rows = users;

    this.allRoles = roles;
  }

  newUser() {
    this.editingUserName = null;
    this.sourceUser = null;
    this.editedUser = this.userEditor.newUser(this.allRoles);
    this.editorModal.show();
  }

  editUser(row: UserEditor) {
    this.editingUserName = { name: row.userName };
    this.sourceUser = row;
    this.editedUser = this.userEditor.editUser(row, this.allRoles);
    this.editorModal.show();
  }

  deleteUser(row: UserEditor) {
    this.userService.deleteUser(row)
      .subscribe(results => {
        this.rowsCache = this.rowsCache.filter(item => item !== row);
        this.rows = this.rows.filter(item => item !== row);
      });
  }

  deleteUserHelper(row: UserEditor) {
    throw new Error('Method not implemented.');
  }

  onEditorModalHidden() {
    this.editingUserName = null;
    this.userEditor.resetForm(true);
  }
}
