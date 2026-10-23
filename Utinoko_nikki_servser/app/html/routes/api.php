<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers;

Route::get('/user', function (Request $request) {
    return $request->user();
})->middleware('auth:sanctum');

Route::get('/items', [Controllers\ItemController::class, 'index']);
Route::get('/elderUser', [Controllers\ElderUserController::class, 'index']);
Route::post('/elderUser', [Controllers\ElderUserController::class, 'store']);
Route::get('/familyUser', [Controllers\FamilyUserController::class, 'index']);
Route::post('/familyUser', [Controllers\FamilyUserController::class, 'store']);
Route::get('/familyLink', [Controllers\FamilyLinkController::class, 'index']);
Route::post('/familyLink', [Controllers\FamilyLinkController::class, 'store']);
