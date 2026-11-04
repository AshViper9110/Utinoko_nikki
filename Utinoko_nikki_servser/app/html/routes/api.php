<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers;

Route::get('/user', function (Request $request) {
    return $request->user();
})->middleware('auth:sanctum');

Route::get('/items', [Controllers\ItemController::class, 'index']);
Route::get('/elder-user', [Controllers\ElderUserController::class, 'index']);
Route::post('/elder-user', [Controllers\ElderUserController::class, 'store']);
Route::get('/family-user', [Controllers\FamilyUserController::class, 'index']);
Route::post('/family-user', [Controllers\FamilyUserController::class, 'store']);
Route::get('/family-link', [Controllers\FamilyLinkController::class, 'index']);
Route::post('/family-link', [Controllers\FamilyLinkController::class, 'store']);
Route::get('/games', [Controllers\GameController::class, 'index']);
Route::post('/games', [Controllers\GameController::class, 'store']);
