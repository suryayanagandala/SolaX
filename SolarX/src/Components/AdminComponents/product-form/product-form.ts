import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../Shared/product';
import { MaterialModule } from '../../../MaterialModule';
import { Product } from '../../../Models/Models';
import { CommonModule } from '@angular/common';
import { MatFormFieldControl } from '@angular/material/form-field';
import { CategorieService } from '../../../Shared/categories';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [ReactiveFormsModule, MaterialModule,CommonModule],
  templateUrl: './product-form.html',
  styleUrls: ['./product-form.scss'],
})
export class ProductForm implements OnInit {
  productForm!: FormGroup;
  productId: number | null = null;
  isEditMode = false;
  categories: any[] = [];

 types = [
  { value: 'Mono', label: 'Mono' },
  { value: 'Poly', label: 'Poly' },
  { value: 'PERC', label: 'PERC' },
  { value: 'TOPCon', label: 'TOPCon' },
  { value: 'HJT', label: 'HJT' },
  { value: 'TFSP', label: 'TFSP' },
  { value: 'Bifacial', label: 'Bifacial' },
  {value:'Tabular',label:'Tabular'},
  {value:'SMF',label:'SMF'},
  {value:'Lithium-Ion',label:'Lithium-Ion'},
  {value:'Lithium-Ion',label:'Flat Plate'},
  {value:'',label:''}

];
        

  constructor(
    private service: ProductService,
    private toastr: ToastrService,
    private builder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute, private categoryService:CategorieService
  ) {}

  ngOnInit(): void {
    this.initForm();

    this.productForm.get('capacityValue')?.valueChanges.subscribe(val => 
      { if (val) 
        { this.productForm.get('capacity')?.setValue(`${val}W`, { emitEvent: false }); 
    } else 
      { this.productForm.get('capacity')?.setValue('', { emitEvent: false }); 
    } });

    this.route.queryParams.subscribe(params => {
      if (params['productId']) {
        this.productId = Number(params['productId']);
        this.isEditMode = true;
        this.loadProductDetails(this.productId);
      }
    });

    this.categoryService.getAllCategories().subscribe((res)=>{
      if(res.status){
        this.categories=res.data??[];

      }
      else{
        console.log(res.error);
        
      }
    })
  }

  initForm() {
    this.productForm = this.builder.group({
      productName: this.builder.control('', Validators.required),
      price: this.builder.control('', Validators.compose([Validators.required,Validators.min(0)])),
      description: this.builder.control(
        '',
        Validators.compose([Validators.required, Validators.minLength(30)])
      ),
      capacity: this.builder.control('', Validators.required),
      capacityValue: this.builder.control('', Validators.compose([Validators.required,Validators.min(0)])),
      stock: this.builder.control('', Validators.compose([Validators.required,Validators.min(0)])),
      type: this.builder.control<string | null>(null, Validators.required),
      brand: this.builder.control('', Validators.required),
      warranty_Years: this.builder.control('', Validators.compose([Validators.required,Validators.min(0)])),
      // isActive: [false],
     categoryId: this.builder.control<number | null>(null, Validators.required)
    });
  }

  loadProductDetails(id: number) {
    this.service.getProductDetalById(id).subscribe(res => {
      if (res.status) {
        this.productForm.patchValue(res.data ?? {});
      }
    });
  }

  submit() {
    if (this.productForm.invalid) {
      this.toastr.error('Incorrect Form');
      return;
    }

 

    if (this.productId && this.isEditMode) {
      this.update();
    } else {
      this.create();
    }
  }

  create() {
    this.service.createProduct(this.productForm.value).subscribe(res => {
      if (res.status) {
        this.toastr.success('Successfully added');
        this.router.navigate(['admin/product-details']); 
      } else {
        this.toastr.error('Failed to add product');
        console.log(res.error);
        
      }
    });
  }

  update() {
    this.service.updateProduct(this.productId!, this.productForm.value).subscribe(res => {
      if (res.status) {
        this.toastr.success('Successfully updated');
        console.log(res.data);
        
        this.router.navigate(['admin/product-details']);
      } else {
        this.toastr.error('Failed to update product');
        console.log(res.error);
        
      }
    });
  }
}
